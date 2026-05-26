using HealthPlatform.ClinicalIntelligence.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthPlatform.ClinicalIntelligence.Api.Data;

public sealed class ClinicalDbContext : DbContext
{
    public ClinicalDbContext(DbContextOptions<ClinicalDbContext> options) : base(options) { }

    public DbSet<AiSuggestion> AiSuggestions => Set<AiSuggestion>();
    public DbSet<MedicalCode> MedicalCodes => Set<MedicalCode>();
    public DbSet<ConflictDetection> ConflictDetections => Set<ConflictDetection>();
    public DbSet<RiskScore> RiskScores => Set<RiskScore>();
    public DbSet<IntakeResponse> IntakeResponses => Set<IntakeResponse>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("clinical_intelligence");

        modelBuilder.Entity<AiSuggestion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.PatientId, e.CreatedAt });
        });

        modelBuilder.Entity<MedicalCode>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.PatientId, e.CodeSystem, e.Code });
            entity.HasOne(e => e.Suggestion).WithMany().HasForeignKey(e => e.SuggestionId);
        });

        modelBuilder.Entity<ConflictDetection>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.PatientId, e.DetectedAt });
        });

        modelBuilder.Entity<RiskScore>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.PatientId, e.CalculatedAt });
        });

        modelBuilder.Entity<IntakeResponse>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.PatientId, e.StartedAt });
        });
    }
}
