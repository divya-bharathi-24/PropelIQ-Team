using System.ComponentModel.DataAnnotations;

namespace HealthPlatform.Shared.Data.Entities;

public sealed class InsuranceDetail
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    [MaxLength(100)]
    public required string ProviderName { get; set; }

    [MaxLength(50)]
    public required string PolicyNumber { get; set; }

    [MaxLength(50)]
    public string? GroupNumber { get; set; }

    public DateOnly EffectiveDate { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public bool IsPrimary { get; set; }
}
