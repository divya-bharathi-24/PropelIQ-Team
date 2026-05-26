using System.ComponentModel.DataAnnotations;

namespace HealthPlatform.Document.Api.Data.Entities;

public sealed class DocumentVersion
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public Document Document { get; set; } = null!;

    public int VersionNumber { get; set; }

    [MaxLength(1024)]
    public required string StoragePath { get; set; }

    public long FileSizeBytes { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
