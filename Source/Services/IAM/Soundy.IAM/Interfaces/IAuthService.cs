using Soundy.IAM.Dto;
using Soundy.IAM.Entities;

namespace Soundy.IAM.Interfaces;

public interface IAuthService
{
    Task<AuthResult> SignInAsync(string username, string password);
    Task<bool> SignUpAsync(string username, string email, string password);
    Task<bool> SignOutAsync(string refreshToken);
    Task<TokenValidationResult> ValidateTokenAsync(string token);
    Task<AuthResult> RefreshTokenAsync(string refreshToken);
    Task<(bool Success, string? ErrorMessage)> UpdateUserDataAsync(string userId, string username, string? email = null);
    Task<(bool Success, string? ErrorMessage)> DeleteUserAsync(string userId);
}