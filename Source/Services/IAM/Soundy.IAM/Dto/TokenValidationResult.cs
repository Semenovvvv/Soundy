namespace Soundy.IAM.Dto;

public class TokenValidationResult
{
    public bool IsValid { get; set; }
    public string? UserId { get; set; }
    public string? ErrorMessage { get; set; }
} 