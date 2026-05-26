using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthPlatform.Admin.Api.Data.Entities;

/// <summary>
/// Immutable audit event record. No UPDATE or DELETE operations allowed at the database level.
/// Partitioned by timestamp with monthly rotation.
/// </summary>
public sealed class AuditEvent
{
    public Guid EventId { get; set; }
    public Guid ActorId { get; set; }

    [MaxLength(100)]
    public required string Action { get; set; }

    [MaxLength(100)]
    public required string ResourceType { get; set; }

    public Guid ResourceId { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "jsonb")]
    public string? PayloadJson { get; set; }

    [MaxLength(45)]
    public string? IpAddress { get; set; }

    [MaxLength(512)]
    public string? UserAgent { get; set; }

    [MaxLength(100)]
    public string? CorrelationId { get; set; }
}
