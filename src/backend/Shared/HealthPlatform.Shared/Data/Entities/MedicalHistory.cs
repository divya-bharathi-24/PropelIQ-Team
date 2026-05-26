using System.ComponentModel.DataAnnotations;

namespace HealthPlatform.Shared.Data.Entities;

public sealed class MedicalHistory
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    [MaxLength(200)]
    public required string Condition { get; set; }

    [MaxLength(200)]
    public string? Diagnosis { get; set; }

    public DateOnly? DiagnosedDate { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
}
