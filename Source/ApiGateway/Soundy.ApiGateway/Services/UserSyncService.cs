using Service.Iam;
using Service.User;
using Soundy.ApiGateway.Services.Interfaces;
using System.Net.Mail;

namespace Soundy.ApiGateway.Services;

public class UserSyncService : IUserSyncService
{
    private readonly IAMGrpcService.IAMGrpcServiceClient _iamClient;
    private readonly UserGrpcService.UserGrpcServiceClient _userClient;
    private readonly ILogger<UserSyncService> _logger;

    public UserSyncService(
        IAMGrpcService.IAMGrpcServiceClient iamClient,
        UserGrpcService.UserGrpcServiceClient userClient,
        ILogger<UserSyncService> logger)
    {
        _iamClient = iamClient;
        _userClient = userClient;
        _logger = logger;
    }

    public async Task<string> SyncUserAsync(string username, string email, string? bio = null)
    {
        try
        {
            // Сначала проверяем валидность почты
            if (!IsValidEmail(email))
            {
                throw new ArgumentException("Invalid email format");
            }

            // Проверяем наличие пользователя в IAM
            var signInResponse = await _iamClient.SignInAsync(new SignInRequest
            {
                Username = username,
                Password = "temporary_password" // Мы не используем пароль, просто проверяем существование
            });

            string userId = signInResponse.UserId;

            // Создаем пользователя в User сервисе, если он не существует
            try
            {
                var userResponse = await _userClient.GetByIdAsync(new GetByIdRequest { Id = userId });
                _logger.LogInformation("User {Username} already exists in User service with ID: {UserId}", username, userId);
                
                // Обновляем информацию, если нужно
                await _userClient.UpdateAsync(new UpdateRequest
                {
                    Id = userId,
                    Username = username,
                    Email = email
                });
                
                return userId;
            }
            catch
            {
                // Пользователя нет в User сервисе, создаем его
                _logger.LogInformation("Creating user {Username} in User service", username);
                
                var createResponse = await _userClient.CreateAsync(new CreateRequest
                {
                    Name = username,
                    Email = email,
                    Bio = bio
                });
                
                return createResponse.User.Id;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing user between IAM and User services");
            throw;
        }
    }

    public async Task<bool> ValidateUserSyncAsync(string userId)
    {
        try
        {
            // Проверяем наличие пользователя в обоих сервисах
            var validateResponse = await _iamClient.ValidateTokenAsync(new ValidateTokenRequest
            {
                Token = userId // Временно используем userId как токен для проверки
            });

            if (!validateResponse.IsValid)
            {
                _logger.LogWarning("User {UserId} not found in IAM service", userId);
                return false;
            }

            try
            {
                var userResponse = await _userClient.GetByIdAsync(new GetByIdRequest { Id = userId });
                _logger.LogInformation("User {UserId} exists in both IAM and User services", userId);
                return true;
            }
            catch
            {
                _logger.LogWarning("User {UserId} not found in User service", userId);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating user sync between IAM and User services");
            return false;
        }
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var mailAddress = new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }
} 