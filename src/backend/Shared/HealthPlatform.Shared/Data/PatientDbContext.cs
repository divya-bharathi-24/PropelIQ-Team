using HealthPlatform.Shared.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthPlatform.Shared.Data;

public sealed class PatientDbContext : DbContext
{
    public PatientDbContext(DbContextOptions<PatientDbContext> options) : base(options) { }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<PatientContact> PatientContacts => Set<PatientContact>();
    public DbSet<InsuranceDetail> InsuranceDetails => Set<InsuranceDetail>();
    public DbSet<MedicalHistory> MedicalHistories => Set<MedicalHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("patient");

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.LastName, e.DateOfBirth });
            entity.Property(e => e.FirstName).IsRequired();
            entity.Property(e => e.LastName).IsRequired();
        });

        modelBuilder.Entity<PatientContact>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Patient).WithMany(p => p.Contacts).HasForeignKey(e => e.PatientId);
        });

        modelBuilder.Entity<InsuranceDetail>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Patient).WithMany(p => p.InsuranceDetails).HasForeignKey(e => e.PatientId);
        });

        modelBuilder.Entity<MedicalHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Patient).WithMany(p => p.MedicalHistories).HasForeignKey(e => e.PatientId);
        });
    }
}
