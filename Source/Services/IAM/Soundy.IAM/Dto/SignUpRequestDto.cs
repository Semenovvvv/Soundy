using Microsoft.AspNetCore.SignalR.Protocol;

namespace Soundy.IAM.Dto
{
    public class SignUpRequestDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
