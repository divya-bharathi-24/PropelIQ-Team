using System.ComponentModel.DataAnnotations;

namespace HealthPlatform.ClinicalIntelligence.Api.Data.Entities;

public sealed class RiskScore
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }

    [MaxLength(50)]
    public required string RiskType { get; set; }

    public double Score { get; set; }

    [MaxLength(20)]
    public required string Category { get; set; }

    [MaxLength(1000)]
    public string? FactorsJson { get; set; }

    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
}
