using HealthPlatform.Admin.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthPlatform.Admin.Api.Data;

public sealed class AuditDbContext : DbContext
{
    public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options) { }

    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("audit");

        modelBuilder.Entity<AuditEvent>(entity =>
        {
            entity.HasKey(e => e.EventId);

            // Partitioned index on (Timestamp, ResourceType) for efficient querying
            entity.HasIndex(e => new { e.Timestamp, e.ResourceType });
            entity.HasIndex(e => e.ActorId);
            entity.HasIndex(e => e.ResourceId);
            entity.HasIndex(e => e.CorrelationId);

            entity.Property(e => e.Action).IsRequired();
            entity.Property(e => e.ResourceType).IsRequired();
        });
    }

    /// <summary>
    /// Override SaveChanges to enforce append-only behavior.
    /// Rejects modifications and deletions of AuditEvent entities.
    /// </summary>
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        EnforceAppendOnly();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        EnforceAppendOnly();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void EnforceAppendOnly()
    {
        var modifiedOrDeleted = ChangeTracker.Entries<AuditEvent>()
            .Where(e => e.State is EntityState.Modified or EntityState.Deleted)
            .ToList();

        if (modifiedOrDeleted.Count > 0)
        {
            throw new InvalidOperationException(
                "Audit events are immutable. UPDATE and DELETE operations are not permitted.");
        }
    }
}
