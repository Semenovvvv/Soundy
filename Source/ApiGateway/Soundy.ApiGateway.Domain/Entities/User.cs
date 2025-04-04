using Soundy.SharedLibrary.Enums;

namespace Soundy.ApiGateway.Domain.Entities
{
    /// <summary> Пользователь </summary>
    public class User
    {
        /// <summary> Идентификатор </summary>
        public required Guid Id { get; set; }

        /// <summary> Электронная почта </summary>
        public required string Email { get; set; }

        /// <summary> Логин </summary>
        public required string Login { get; set; }

        /// <summary> Телефонный номер </summary>
        public required string Phone { get; set; }

        /// <summary> Пароль </summary>
        public required string PasswordHash { get; set; }

        /// <summary> Роль </summary>
        public required RoleType Role { get; set; }
    }
}
