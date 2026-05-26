using System.ComponentModel.DataAnnotations;

namespace HealthPlatform.Document.Api.Data.Entities;

public sealed class DocumentMetadata
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public Document Document { get; set; } = null!;

    [MaxLength(100)]
    public string? DocumentType { get; set; }

    public DateOnly? DocumentDate { get; set; }

    [MaxLength(200)]
    public string? ProviderName { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? Category { get; set; }
}
