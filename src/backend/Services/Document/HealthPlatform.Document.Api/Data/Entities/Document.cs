using System.ComponentModel.DataAnnotations;

namespace HealthPlatform.Document.Api.Data.Entities;

public enum ProcessingStatus
{
    Uploaded,
    Queued,
    Processing,
    Completed,
    Failed
}

public sealed class Document
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid UploadedByUserId { get; set; }

    [MaxLength(256)]
    public required string FileName { get; set; }

    [MaxLength(100)]
    public required string MimeType { get; set; }

    /// <summary>File stored externally; this is the storage reference path (not inline BLOB).</summary>
    [MaxLength(1024)]
    public required string StoragePath { get; set; }

    public long FileSizeBytes { get; set; }

    [MaxLength(64)]
    public string? Sha256Hash { get; set; }

    public ProcessingStatus ProcessingStatus { get; set; } = ProcessingStatus.Uploaded;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<DocumentVersion> Versions { get; set; } = new List<DocumentVersion>();
    public DocumentMetadata? Metadata { get; set; }
    public OcrResult? OcrResult { get; set; }
}
