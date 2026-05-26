using System.ComponentModel.DataAnnotations;

namespace HealthPlatform.Shared.Data.Entities;

public sealed class Patient
{
    public Guid Id { get; set; }

    [MaxLength(100)]
    public required string FirstName { get; set; }

    [MaxLength(100)]
    public required string LastName { get; set; }

    public required DateOnly DateOfBirth { get; set; }

    [MaxLength(20)]
    public string? Gender { get; set; }

    /// <summary>PII — encrypted at rest via AES-256 value converter.</summary>
    [MaxLength(20)]
    public string? Ssn { get; set; }

    /// <summary>PII — encrypted at rest via AES-256 value converter.</summary>
    [MaxLength(256)]
    public string? Email { get; set; }

    /// <summary>PII — encrypted at rest via AES-256 value converter.</summary>
    [MaxLength(20)]
    public string? Phone { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PatientContact> Contacts { get; set; } = new List<PatientContact>();
    public ICollection<InsuranceDetail> InsuranceDetails { get; set; } = new List<InsuranceDetail>();
    public ICollection<MedicalHistory> MedicalHistories { get; set; } = new List<MedicalHistory>();
}
