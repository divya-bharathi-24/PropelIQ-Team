using HealthPlatform.Appointment.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthPlatform.Appointment.Api.Data;

public sealed class AppointmentDbContext : DbContext
{
    public AppointmentDbContext(DbContextOptions<AppointmentDbContext> options) : base(options) { }

    public DbSet<Entities.Appointment> Appointments => Set<Entities.Appointment>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
    public DbSet<ProviderSchedule> ProviderSchedules => Set<ProviderSchedule>();
    public DbSet<SlotSwapRequest> SlotSwapRequests => Set<SlotSwapRequest>();
    public DbSet<CalendarConnection> CalendarConnections => Set<CalendarConnection>();
    public DbSet<InsuranceDetails> InsuranceDetails => Set<InsuranceDetails>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("appointment");

        modelBuilder.Entity<Entities.Appointment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ProviderId, e.ScheduledAt });
            entity.HasIndex(e => new { e.PatientId, e.Status });
            entity.HasOne(e => e.TimeSlot).WithMany().HasForeignKey(e => e.TimeSlotId);
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20);
        });

        modelBuilder.Entity<TimeSlot>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ProviderId, e.StartTime, e.EndTime }).IsUnique();
            entity.HasOne(e => e.Schedule).WithMany(s => s.TimeSlots).HasForeignKey(e => e.ScheduleId);

            // No-overlap constraint per provider
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_TimeSlot_NoOverlap",
                "\"StartTime\" < \"EndTime\""));
        });

        modelBuilder.Entity<ProviderSchedule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ProviderId, e.ScheduleDate });
        });

        modelBuilder.Entity<SlotSwapRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.PatientId, e.Status });
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20);
        });

        modelBuilder.Entity<CalendarConnection>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.PatientId, e.Provider }).IsUnique();
            entity.HasIndex(e => e.IcsFeedToken).IsUnique();
        });

        modelBuilder.Entity<InsuranceDetails>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.PatientId, e.IsPrimary });
        });
    }
}
