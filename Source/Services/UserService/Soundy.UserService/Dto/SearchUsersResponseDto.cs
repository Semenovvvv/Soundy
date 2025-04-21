namespace Soundy.UserService.Dto
{
    public class SearchUsersResponseDto
    {
        public IEnumerable<UserDto> Users { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
