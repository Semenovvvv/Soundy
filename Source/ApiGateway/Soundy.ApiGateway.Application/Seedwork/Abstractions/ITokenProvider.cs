using Soundy.ApiGateway.Domain.Entities;

namespace Soundy.ApiGateway.Application.Seedwork.Abstractions
{
    /// <summary>
    /// Контракт токен провайдера
    /// </summary>
    internal interface ITokenProvider
    {
        /// <summary>
        /// Сгенерировать токен
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns>Токен</returns>
        string GenerateToken(User user);
    }
}
