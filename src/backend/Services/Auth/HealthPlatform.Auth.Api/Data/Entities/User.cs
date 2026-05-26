using System.ComponentModel.DataAnnotations;

namespace HealthPlatform.Auth.Api.Data.Entities;

public sealed class User
{
    public Guid Id { get; set; }

    [MaxLength(256)]
    public required string Email { get; set; }

    [MaxLength(256)]
    public required string PasswordHash { get; set; }

    [MaxLength(100)]
    public required string FirstName { get; set; }

    [MaxLength(100)]
    public required string LastName { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    [MaxLength(30)]
    public string Status { get; set; } = "pending_verification";

    public bool IsActive { get; set; } = true;
    public bool ForcePasswordChange { get; set; }
    public Guid? CreatedByStaffId { get; set; }

    [MaxLength(512)]
    public string? VerificationToken { get; set; }
    public DateTime? VerificationTokenExpiresAt { get; set; }
    public int VerificationEmailsSentInLastHour { get; set; }
    public DateTime? LastVerificationEmailSentAt { get; set; }

    [MaxLength(512)]
    public string? ProfilePhotoPath { get; set; }

    [ConcurrencyCheck]
    public uint RowVersion { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<LoginAttempt> LoginAttempts { get; set; } = new List<LoginAttempt>();
}
