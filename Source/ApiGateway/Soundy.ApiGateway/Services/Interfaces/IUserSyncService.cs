namespace Soundy.ApiGateway.Services.Interfaces;

public interface IUserSyncService
{
    /// <summary>
    /// Синхронизирует пользователя между IAM и User сервисами
    /// </summary>
    /// <param name="username">Имя пользователя</param>
    /// <param name="email">Email пользователя</param>
    /// <param name="bio">Информация о пользователе (опционально)</param>
    /// <returns>ID пользователя в случае успеха</returns>
    Task<string> SyncUserAsync(string username, string email, string? bio = null);
    
    /// <summary>
    /// Проверяет соответствие пользователя в IAM и User сервисах
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <returns>true, если пользователь существует в обоих сервисах</returns>
    Task<bool> ValidateUserSyncAsync(string userId);
} 