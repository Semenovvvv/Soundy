namespace Soundy.UserService.Dto
{
    public class UpdateUserResponseDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
