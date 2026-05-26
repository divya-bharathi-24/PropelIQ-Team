using System.ComponentModel.DataAnnotations;

namespace HealthPlatform.Notification.Api.Data.Entities;

public sealed class DeliveryAttempt
{
    public Guid Id { get; set; }
    public Guid NotificationLogId { get; set; }
    public NotificationLog NotificationLog { get; set; } = null!;

    public int AttemptNumber { get; set; }

    [MaxLength(50)]
    public required string Status { get; set; }

    [MaxLength(500)]
    public string? ErrorMessage { get; set; }

    [MaxLength(100)]
    public string? ExternalMessageId { get; set; }

    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
}
