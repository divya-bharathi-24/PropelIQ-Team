using System.ComponentModel.DataAnnotations;

namespace HealthPlatform.Auth.Api.Data.Entities;

public sealed class LoginAttempt
{
    public Guid Id { get; set; }

    [MaxLength(256)]
    public required string Email { get; set; }

    public Guid? UserId { get; set; }

    [MaxLength(45)]
    public required string IpAddress { get; set; }

    [MaxLength(512)]
    public string? UserAgent { get; set; }

    [MaxLength(50)]
    public required string EventType { get; set; }

    public bool Successful { get; set; }
    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(256)]
    public string? FailureReason { get; set; }
}
