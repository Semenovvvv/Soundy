namespace Soundy.UserService.Dto
{
    public class SearchResponseDto
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public IEnumerable<UserDto> Users { get; set; }
    }
}
