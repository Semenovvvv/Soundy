namespace Soundy.UserService.Dto
{
    public class SearchUsersRequestDto
    {
        public string Pattern { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
