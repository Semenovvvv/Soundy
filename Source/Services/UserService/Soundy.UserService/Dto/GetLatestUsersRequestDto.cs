namespace Soundy.UserService.Dto
{
    public class GetLatestUsersRequestDto
    {
        /// <summary>
        /// Количество записей для получения (по умолчанию 10)
        /// </summary>
        public int Count { get; set; } = 10;
    }
} 