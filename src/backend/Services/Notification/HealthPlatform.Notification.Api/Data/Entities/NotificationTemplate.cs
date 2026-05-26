using System.ComponentModel.DataAnnotations;

namespace HealthPlatform.Notification.Api.Data.Entities;

public sealed class NotificationTemplate
{
    public Guid Id { get; set; }

    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(50)]
    public required string Channel { get; set; }

    [MaxLength(200)]
    public string? Subject { get; set; }

    public required string BodyTemplate { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
