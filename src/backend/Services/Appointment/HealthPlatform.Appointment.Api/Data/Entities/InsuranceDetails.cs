using System.ComponentModel.DataAnnotations;

namespace HealthPlatform.Appointment.Api.Data.Entities;

public sealed class InsuranceDetails
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }

    [Required, MaxLength(200)]
    public string InsuranceProvider { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string PolicyNumber { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? GroupNumber { get; set; }

    [Required, MaxLength(50)]
    public string MemberId { get; set; } = string.Empty;

    [MaxLength(30)]
    public string CoverageStatus { get; set; } = "Unknown"; // Active-Covered, Active-Partial, Inactive, Unable to Verify

    public decimal? CopayEstimate { get; set; }

    [MaxLength(500)]
    public string? Limitations { get; set; }

    public bool IsPrimary { get; set; } = true;
    public DateTime? LastVerifiedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
