namespace Soundy.UserService.Dto
{
    public class UpdateUserRequestDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
