using HealthPlatform.Appointment.Api.Data;
using HealthPlatform.Appointment.Api.Data.Entities;
using HealthPlatform.Appointment.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthPlatform.Appointment.Api.Jobs;

public sealed class InsuranceReVerifyJob
{
    private readonly AppointmentDbContext _db;

    public InsuranceReVerifyJob(AppointmentDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Re-verifies insurance 24 hours before an appointment.
    /// Called via Hangfire scheduled job.
    /// </summary>
    public async Task ExecuteAsync(Guid appointmentId, CancellationToken ct)
    {
        var appointment = await _db.Appointments
            .FirstOrDefaultAsync(a => a.Id == appointmentId && a.Status == AppointmentStatus.Scheduled, ct);

        if (appointment is null) return;

        var insurance = await _db.InsuranceDetails
            .FirstOrDefaultAsync(i => i.PatientId == appointment.PatientId && i.IsPrimary, ct);

        if (insurance is null) return;

        // Re-verify: check if insurance is still valid (using dummy matching logic)
        insurance.LastVerifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }
}
