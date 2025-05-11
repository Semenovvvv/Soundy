﻿namespace Soundy.IAM.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public bool Revoked { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ReplacedByToken { get; set; }
}