using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Soundy.IAM.Configurations;
using Soundy.IAM.DataAccess;
using Soundy.IAM.Dto;
using Soundy.IAM.Entities;
using Soundy.IAM.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Soundy.IAM.Services;

public class AuthService : IAuthService
{
    private readonly IamDbContext _dbContext;
    private readonly JwtConfig _jwtConfig;

    public AuthService(IamDbContext dbContext, IOptions<JwtConfig> jwtOptions)
    {
        _dbContext = dbContext;
        _jwtConfig = jwtOptions.Value;
    }

    public async Task<AuthResult> SignInAsync(string username, string password)
    {
        var user = await _dbContext.Users
            .SingleOrDefaultAsync(u => u.Username == username);

        if (user == null || !VerifyPasswordHash(password, user.PasswordHash))
        {
            return new AuthResult
            {
                Success = false,
                ErrorMessage = "Invalid username or password"
            };
        }

        // Generate tokens
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();

        // Save refresh token to database
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenExpirationDays),
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.RefreshTokens.Add(refreshTokenEntity);
        await _dbContext.SaveChangesAsync();

        return new AuthResult
        {
            Success = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            UserId = user.Id.ToString()
        };
    }

    public async Task<bool> SignUpAsync(string username, string email, string password)
    {
        // Check if user already exists
        var userExists = await _dbContext.Users.AnyAsync(u => 
            u.Username == username || u.Email == email);

        if (userExists)
        {
            return false;
        }

        // Create new user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            PasswordHash = HashPassword(password),
            CreatedAt = DateTime.UtcNow
        };

        // Add user to database
        _dbContext.Users.Add(user);

        // Assign "User" role by default
        var userRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "User");
        if (userRole == null)
        {
            userRole = new Role { Id = Guid.NewGuid(), Name = "User" };
            _dbContext.Roles.Add(userRole);
        }

        _dbContext.UserRoles.Add(new UserRole 
        { 
            UserId = user.Id, 
            RoleId = userRole.Id 
        });

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SignOutAsync(string refreshToken)
    {
        var token = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == refreshToken);

        if (token == null)
        {
            return false;
        }

        // Revoke the token
        token.Revoked = true;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<Dto.TokenValidationResult> ValidateTokenAsync(string token)
    {
        var result = new Dto.TokenValidationResult { IsValid = false };

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtConfig.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtConfig.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

            // Check if user exists
            var userExists = await _dbContext.Users.AnyAsync(u => u.Id.ToString() == userId);
            if (!userExists)
            {
                return result;
            }

            result.IsValid = true;
            result.UserId = userId;
            return result;
        }
        catch
        {
            return result;
        }
    }

    public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
    {
        var token = await _dbContext.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == refreshToken && !t.Revoked && t.ExpiresAt > DateTime.UtcNow);

        if (token == null)
        {
            return new AuthResult
            {
                Success = false,
                ErrorMessage = "Invalid or expired refresh token"
            };
        }

        // Generate new tokens
        var accessToken = GenerateAccessToken(token.User);
        var newRefreshToken = GenerateRefreshToken();

        // Revoke the old refresh token
        token.Revoked = true;
        token.ReplacedByToken = newRefreshToken;

        // Save new refresh token
        var refreshTokenEntity = new RefreshToken
        {
            Token = newRefreshToken,
            UserId = token.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenExpirationDays),
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.RefreshTokens.Add(refreshTokenEntity);
        await _dbContext.SaveChangesAsync();

        return new AuthResult
        {
            Success = true,
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            UserId = token.UserId.ToString()
        };
    }

    public async Task<(bool Success, string? ErrorMessage)> UpdateUserDataAsync(string userId, string username, string? email = null)
    {
        try
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            
            if (user == null)
            {
                return (false, "User not found");
            }

            // Обновляем имя пользователя
            user.Username = username;
            
            // Больше не обновляем email, даже если он был передан
            // if (email != null)
            // {
            //     // Проверяем, не занят ли email другим пользователем
            //     var emailExists = await _dbContext.Users
            //         .AnyAsync(u => u.Email == email && u.Id != user.Id);
            //         
            //     if (emailExists)
            //     {
            //         return (false, "Email is already in use by another user");
            //     }
            //     
            //     user.Email = email;
            // }
            
            await _dbContext.SaveChangesAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Error updating user data: {ex.Message}");
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteUserAsync(string userId)
    {
        try
        {
            // Проверяем существование пользователя
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user == null)
            {
                return (false, "User not found");
            }

            // Удаляем связанные refresh токены
            var refreshTokens = await _dbContext.RefreshTokens
                .Where(t => t.UserId.ToString() == userId)
                .ToListAsync();
            
            if (refreshTokens.Any())
            {
                _dbContext.RefreshTokens.RemoveRange(refreshTokens);
            }

            // Удаляем связи с ролями
            var userRoles = await _dbContext.UserRoles
                .Where(ur => ur.UserId.ToString() == userId)
                .ToListAsync();
            
            if (userRoles.Any())
            {
                _dbContext.UserRoles.RemoveRange(userRoles);
            }

            // Удаляем пользователя
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Error deleting user: {ex.Message}");
        }
    }

    #region Helper Methods

    private string GenerateAccessToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
        
        // Get user roles
        var roles = _dbContext.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .Join(_dbContext.Roles,
                  ur => ur.RoleId,
                  r => r.Id,
                  (ur, r) => r.Name)
            .ToList();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

        // Add role claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.AccessTokenExpirationMinutes),
            Issuer = _jwtConfig.Issuer,
            Audience = _jwtConfig.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPasswordHash(string password, string storedHash)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        var hash = Convert.ToBase64String(hashedBytes);
        return hash == storedHash;
    }

    #endregion
}