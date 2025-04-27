namespace Soundy.UserService.Dto
{
    public class UpdateRequestDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
