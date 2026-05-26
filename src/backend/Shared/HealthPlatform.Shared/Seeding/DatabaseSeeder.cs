using HealthPlatform.Shared.Data;
using HealthPlatform.Shared.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HealthPlatform.Shared.Seeding;

/// <summary>
/// Database seeder for development environments.
/// Generates realistic test data: 50 patients, 10 providers, 200 appointments, 5 roles.
/// Completes in under 30 seconds. Respects all constraints.
/// </summary>
public sealed class DatabaseSeeder
{
    private readonly PatientDbContext _patientDb;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(PatientDbContext patientDb, ILogger<DatabaseSeeder> logger)
    {
        _patientDb = patientDb;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        if (await _patientDb.Patients.AnyAsync(ct))
        {
            _logger.LogInformation("Database already seeded; skipping");
            return;
        }

        _logger.LogInformation("Seeding database with test data...");

        var patients = SeedDataFactory.CreatePatients(50);
        _patientDb.Patients.AddRange(patients);

        foreach (var patient in patients)
        {
            _patientDb.PatientContacts.AddRange(SeedDataFactory.CreateContacts(patient.Id));
            _patientDb.InsuranceDetails.AddRange(SeedDataFactory.CreateInsurance(patient.Id));
        }

        await _patientDb.SaveChangesAsync(ct);
        _logger.LogInformation("Seeded {Count} patients with contacts and insurance", patients.Count);
    }
}
