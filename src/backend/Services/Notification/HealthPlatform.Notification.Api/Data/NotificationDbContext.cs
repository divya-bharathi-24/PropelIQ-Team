using HealthPlatform.Notification.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthPlatform.Notification.Api.Data;

public sealed class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
    public DbSet<NotificationLog> NotificationLogs => Set<NotificationLog>();
    public DbSet<DeliveryAttempt> DeliveryAttempts => Set<DeliveryAttempt>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("notification");

        modelBuilder.Entity<NotificationTemplate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<NotificationLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.PatientId, e.Channel, e.Status });
            entity.HasIndex(e => e.ArchiveAfter);
            entity.HasOne(e => e.Template).WithMany().HasForeignKey(e => e.TemplateId);
        });

        modelBuilder.Entity<DeliveryAttempt>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.NotificationLog).WithMany(n => n.DeliveryAttempts).HasForeignKey(e => e.NotificationLogId);
        });
    }
}
