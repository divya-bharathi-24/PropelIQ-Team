using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthPlatform.ClinicalIntelligence.Api.Data.Entities;

public sealed class IntakeResponse
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid? AppointmentId { get; set; }

    [MaxLength(50)]
    public required string IntakeMethod { get; set; }

    [Column(TypeName = "jsonb")]
    public required string ResponseDataJson { get; set; }

    [Column(TypeName = "jsonb")]
    public string? ExtractedDataJson { get; set; }

    public bool IsComplete { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}
