using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthPlatform.ClinicalIntelligence.Api.Data.Entities;

public sealed class AiSuggestion
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }

    [MaxLength(50)]
    public required string SuggestionType { get; set; }

    [Column(TypeName = "jsonb")]
    public required string OutputJson { get; set; }

    public double ConfidenceScore { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; }

    public Guid? ReviewedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }
}
