using System.ComponentModel.DataAnnotations;

namespace HealthPlatform.Notification.Api.Data.Entities;

public sealed class NotificationLog
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid? TemplateId { get; set; }
    public NotificationTemplate? Template { get; set; }

    [MaxLength(50)]
    public required string Channel { get; set; }

    [MaxLength(50)]
    public required string Status { get; set; }

    [MaxLength(500)]
    public string? Subject { get; set; }

    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    /// <summary>TTL archival: records older than ArchiveAfter are eligible for cold storage.</summary>
    public DateTime ArchiveAfter { get; set; }

    public ICollection<DeliveryAttempt> DeliveryAttempts { get; set; } = new List<DeliveryAttempt>();
}
