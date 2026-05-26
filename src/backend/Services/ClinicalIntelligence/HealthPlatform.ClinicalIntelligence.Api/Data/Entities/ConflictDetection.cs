using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthPlatform.ClinicalIntelligence.Api.Data.Entities;

public sealed class ConflictDetection
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }

    [MaxLength(50)]
    public required string ConflictType { get; set; }

    [MaxLength(50)]
    public required string Severity { get; set; }

    [Column(TypeName = "jsonb")]
    public required string ConflictDetailsJson { get; set; }

    [MaxLength(50)]
    public string? ResolutionStatus { get; set; }

    public Guid? ResolvedByUserId { get; set; }
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
}
