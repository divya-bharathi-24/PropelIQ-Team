using HealthPlatform.Document.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthPlatform.Document.Api.Data;

public sealed class DocumentDbContext : DbContext
{
    public DocumentDbContext(DbContextOptions<DocumentDbContext> options) : base(options) { }

    public DbSet<Entities.Document> Documents => Set<Entities.Document>();
    public DbSet<DocumentVersion> DocumentVersions => Set<DocumentVersion>();
    public DbSet<DocumentMetadata> DocumentMetadata => Set<DocumentMetadata>();
    public DbSet<OcrResult> OcrResults => Set<OcrResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("document");

        modelBuilder.Entity<Entities.Document>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.PatientId, e.CreatedAt });
            entity.Property(e => e.ProcessingStatus).HasConversion<string>().HasMaxLength(20);
            entity.HasMany(e => e.Versions).WithOne(v => v.Document).HasForeignKey(v => v.DocumentId);
            entity.HasOne(e => e.Metadata).WithOne(m => m.Document).HasForeignKey<DocumentMetadata>(m => m.DocumentId);
            entity.HasOne(e => e.OcrResult).WithOne(o => o.Document).HasForeignKey<OcrResult>(o => o.DocumentId);
        });

        modelBuilder.Entity<DocumentVersion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.DocumentId, e.VersionNumber }).IsUnique();
        });

        modelBuilder.Entity<OcrResult>(entity =>
        {
            entity.HasKey(e => e.Id);
            // 500KB max payload constraint
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_OcrResult_MaxPayload",
                "octet_length(\"ExtractedData\") <= 512000 OR \"OverflowStoragePath\" IS NOT NULL"));
        });
    }
}
