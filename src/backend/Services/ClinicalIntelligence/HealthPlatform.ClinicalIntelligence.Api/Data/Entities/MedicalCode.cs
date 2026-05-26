using System.ComponentModel.DataAnnotations;

namespace HealthPlatform.ClinicalIntelligence.Api.Data.Entities;

public sealed class MedicalCode
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid? SuggestionId { get; set; }
    public AiSuggestion? Suggestion { get; set; }

    [MaxLength(20)]
    public required string CodeSystem { get; set; }

    [MaxLength(20)]
    public required string Code { get; set; }

    [MaxLength(500)]
    public required string Description { get; set; }

    public double ConfidenceScore { get; set; }
    public bool IsVerified { get; set; }
    public Guid? VerifiedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
