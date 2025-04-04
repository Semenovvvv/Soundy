using Microsoft.EntityFrameworkCore;
using Soundy.ApiGateway.Domain.Entities;

namespace Soundy.ApiGateway.DataAccess.Repositories
{
    /// <summary>
    /// Репозиторий для работы с пользователями
    /// </summary>
    /// <param name="databaseContext"></param>
    internal class UserRepository(DatabaseContext databaseContext)
    {
        private readonly DatabaseContext _databaseContext = databaseContext;

        /// <summary>
        /// Получить пользователя по идентификатору
        /// </summary>
        /// <param name="Id">Идентификатор</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Пользователь</returns>
        public async Task<User?> GetByIdAsync(Guid Id, CancellationToken ct = default) =>
            await databaseContext.Users.FirstOrDefaultAsync(x => x.Id == Id, ct);

        /// <summary>
        /// Получить пользователя по почте
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Пользователь</returns>
        public async Task<User?> GetByEmailAsync(string Email, CancellationToken ct = default) =>
            await databaseContext.Users.FirstOrDefaultAsync(x => x.Email == Email, ct);

        /// <summary>
        /// Добавить пользователя
        /// </summary>
        /// <param name="user">Новый пользователь</param>
        /// <param name="ct">Токен отмены</param>
        public async Task AddAsync(User user, CancellationToken ct = default) =>
            await databaseContext.AddAsync(user, ct);

        /// <summary>
        /// Сохранить изменения
        /// </summary>
        /// <param name="ct">Токен отмены</param>
        public async Task SaveAsync(CancellationToken ct = default) =>
            await databaseContext.SaveChangesAsync(ct);
    }
}
