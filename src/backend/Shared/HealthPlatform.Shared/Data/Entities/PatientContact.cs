using System.ComponentModel.DataAnnotations;

namespace HealthPlatform.Shared.Data.Entities;

public sealed class PatientContact
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    [MaxLength(100)]
    public required string ContactName { get; set; }

    [MaxLength(50)]
    public required string Relationship { get; set; }

    /// <summary>PII — encrypted at rest.</summary>
    [MaxLength(20)]
    public required string Phone { get; set; }

    /// <summary>PII — encrypted at rest.</summary>
    [MaxLength(256)]
    public string? Email { get; set; }

    public bool IsPrimary { get; set; }
}
