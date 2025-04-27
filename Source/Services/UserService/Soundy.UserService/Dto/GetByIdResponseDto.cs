namespace Soundy.UserService.Dto
{
    public class GetByIdResponseDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}