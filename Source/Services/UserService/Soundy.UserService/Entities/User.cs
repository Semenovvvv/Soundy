using Microsoft.AspNetCore.Identity;
using Soundy.SharedLibrary.Enums;

namespace Soundy.UserService.Entities
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class User
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public required Guid Id { get; set; }

        public required Guid AuthenticationId { get; set; }

        /// <summary>
        /// Электронная почта
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// Логин
        /// </summary>
        public required string Login { get; set; }

        /// <summary>
        /// Телефонный номер
        /// </summary>
        public required string Phone { get; set; }

        public required DateTime CreatedAt { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        public required RoleType Role { get; set; }
    }
}
