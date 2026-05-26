using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthPlatform.Document.Api.Data.Entities;

public sealed class OcrResult
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public Document Document { get; set; } = null!;

    /// <summary>
    /// OCR extracted text as JSON. Max 500KB payload; overflow stored as file reference.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string? ExtractedData { get; set; }

    /// <summary>If ExtractedData exceeds 500KB, this stores the file reference.</summary>
    [MaxLength(1024)]
    public string? OverflowStoragePath { get; set; }

    public double ConfidenceScore { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(50)]
    public string? OcrEngine { get; set; }
}
