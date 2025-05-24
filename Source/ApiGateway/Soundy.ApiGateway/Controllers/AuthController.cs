using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Iam;
using Service.User;
using Soundy.ApiGateway.Configurations;
using Soundy.ApiGateway.Services.Interfaces;

namespace Soundy.ApiGateway.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAMGrpcService.IAMGrpcServiceClient _iamClient;
    private readonly UserGrpcService.UserGrpcServiceClient _userClient;
    private readonly IUserSyncService _userSyncService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAMGrpcService.IAMGrpcServiceClient iamClient,
        UserGrpcService.UserGrpcServiceClient userClient,
        IUserSyncService userSyncService,
        ILogger<AuthController> logger)
    {
        _iamClient = iamClient;
        _userClient = userClient;
        _userSyncService = userSyncService;
        _logger = logger;
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
    {
        try
        {
            var response = await _iamClient.SignInAsync(request);
            
            try
            {
                var userResponse = await _userClient.GetByIdAsync(new GetByIdRequest { Id = response.UserId });
                _logger.LogInformation("User exists in both IAM and User services");
            }
            catch
            {
                _logger.LogWarning("User found in IAM but not in User service. Attempting to sync...");
                
                try
                {
                    await _userClient.CreateAsync(new CreateRequest
                    {
                        Id = response.UserId,
                        Name = request.Username,
                        Email = request.Username
                    });
                    _logger.LogInformation("Successfully synchronized user to User service");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to sync user to User service, but continuing login process");
                }
            }

            return Ok(new
            {
                accessToken = response.AccessToken,
                refreshToken = response.RefreshToken,
                userId = response.UserId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during sign in");
            return Unauthorized(new { message = "Invalid username or password" });
        }
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] UserRegistrationRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Username) || 
                string.IsNullOrEmpty(request.Email) || 
                string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Username, email and password are required" });
            }

            var iamResponse = await _iamClient.SignUpAsync(new SignUpRequest
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password
            });

            if (!iamResponse.Success)
            {
                return BadRequest(new { message = "Failed to register user in IAM service" });
            }

            _logger.LogInformation("User successfully registered in IAM service");

            try
            {
                // После регистрации в IAM сразу получаем информацию о пользователе (включая ID)
                var signInResponse = await _iamClient.SignInAsync(new SignInRequest
                {
                    Username = request.Username,
                    Password = request.Password
                });

                string userId = signInResponse.UserId;
                _logger.LogInformation("Retrieved user ID from IAM: {UserId}", userId);

                // Создаем пользователя в User сервисе с тем же ID
                var userResponse = await _userClient.CreateAsync(new CreateRequest
                {
                    Name = request.Username,
                    Email = request.Email,
                    Bio = request.Bio,
                    Id = userId // Используем ID из IAM сервиса
                });

                _logger.LogInformation("User successfully created in User service with ID: {UserId}", userResponse.User.Id);

                return Ok(new
                {
                    message = "User registered successfully",
                    userId = userResponse.User.Id,
                    accessToken = signInResponse.AccessToken,
                    refreshToken = signInResponse.RefreshToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user in User service");
                
                try 
                {
                    var signInResponse = await _iamClient.SignInAsync(new SignInRequest
                    {
                        Username = request.Username,
                        Password = request.Password
                    });

                    return StatusCode(207, new { 
                        message = "User registered in IAM service but failed to create in User service",
                        warning = "User profile is incomplete",
                        userId = signInResponse.UserId,
                        accessToken = signInResponse.AccessToken,
                        refreshToken = signInResponse.RefreshToken
                    });
                }
                catch (Exception signInEx)
                {
                    _logger.LogError(signInEx, "Error during sign in after registration");
                    return StatusCode(207, new { 
                        message = "User registered in IAM service but failed to create in User service and to retrieve tokens",
                        warning = "Please sign in manually"
                    });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during sign up");
            return BadRequest(new { message = "Registration failed: " + ex.Message });
        }
    }

    [HttpPost("signout")]
    [Authorize]
    public async Task<IActionResult> SignOut([FromBody] SignOutRequest request)
    {
        try
        {
            var response = await _iamClient.SignOutAsync(request);
            if (response.Success)
            {
                return Ok(new { message = "Signed out successfully" });
            }
            return BadRequest(new { message = "Sign out failed" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during sign out");
            return BadRequest(new { message = "Sign out failed" });
        }
    }

    [HttpPost("validate-token")]
    public async Task<IActionResult> ValidateToken([FromBody] ValidateTokenRequest request)
    {
        try
        {
            var response = await _iamClient.ValidateTokenAsync(request);
            return Ok(new
            {
                isValid = response.IsValid,
                userId = response.UserId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            return BadRequest(new { message = "Token validation failed" });
        }
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest(new { message = "Refresh token is required" });
            }

            var response = await _iamClient.RefreshTokenAsync(new Service.Iam.RefreshTokenRequest
            {
                RefreshToken = request.RefreshToken
            });

            if (!response.Success)
            {
                return Unauthorized(new { message = response.ErrorMessage });
            }

            return Ok(new
            {
                accessToken = response.AccessToken,
                refreshToken = response.RefreshToken,
                userId = response.UserId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return Unauthorized(new { message = "Token refresh failed" });
        }
    }

    [HttpGet("profile")]
    [JwtAuthorize]
    public async Task<IActionResult> GetUserProfile()
    {
        try
        {
            var userId = this.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var userResponse = await _userClient.GetByIdAsync(new GetByIdRequest { Id = userId });
            
            return Ok(new
            {
                id = userResponse.User.Id,
                name = userResponse.User.Name,
                email = userResponse.User.Email,
                bio = userResponse.User.Bio,
                avatarUrl = userResponse.User.AvatarUrl,
                createdAt = userResponse.User.CreatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user profile");
            return BadRequest(new { message = "Failed to retrieve user profile" });
        }
    }

    [HttpPost("sync")]
    [JwtAuthorize]
    public async Task<IActionResult> SyncUser()
    {
        try
        {
            var userId = this.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            // Получаем информацию о пользователе из IAM по токену
            var validateResponse = await _iamClient.ValidateTokenAsync(new ValidateTokenRequest
            {
                Token = this.GetJwtToken()
            });

            if (!validateResponse.IsValid)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            bool isSynced = await _userSyncService.ValidateUserSyncAsync(userId);
            
            if (isSynced)
            {
                return Ok(new { message = "User is already synchronized between services" });
            }
            else
            {
                // Если пользователь не синхронизирован, то нужно получить его данные из IAM
                // и создать в User сервисе
                // В реальном проекте здесь нужно было бы получить информацию о пользователе из IAM
                // Но для упрощения мы используем заглушку
                
                // Получаем токен пользователя и пробуем по нему извлечь email
                var jwtHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtToken = jwtHandler.ReadJwtToken(this.GetJwtToken());
                
                string email = jwtToken.Claims
                    .FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value ?? "";
                
                string username = jwtToken.Claims
                    .FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value ?? "";

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username))
                {
                    return BadRequest(new { message = "Cannot extract user information from token" });
                }

                await _userClient.CreateAsync(new CreateRequest
                {
                    Name = username,
                    Email = email
                });

                return Ok(new { message = "User successfully synchronized" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing user");
            return BadRequest(new { message = "Failed to sync user" });
        }
    }
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = null!;
}

public class UserRegistrationRequest
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? Bio { get; set; }
} 