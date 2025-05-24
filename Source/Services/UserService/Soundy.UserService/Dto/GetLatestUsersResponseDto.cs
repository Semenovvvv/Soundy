namespace Soundy.UserService.Dto
{
    public class GetLatestUsersResponseDto
    {
        /// <summary>
        /// Список последних зарегистрированных пользователей
        /// </summary>
        public List<UserDto> Users { get; set; } = new List<UserDto>();
    }
} 