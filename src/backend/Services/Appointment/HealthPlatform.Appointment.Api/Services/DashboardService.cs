using HealthPlatform.Appointment.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace HealthPlatform.Appointment.Api.Services;

public sealed class DashboardService
{
    private readonly AppointmentDbContext _db;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(AppointmentDbContext db, ILogger<DashboardService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<object?> GetUpcomingAppointmentsAsync(
        Guid patientId, CancellationToken ct = default)
    {
        try
        {
            var appointments = await _db.Appointments
                .AsNoTracking()
                .Where(a => a.PatientId == patientId &&
                            a.ScheduledAt >= DateTime.UtcNow &&
                            a.Status == Data.Entities.AppointmentStatus.Scheduled)
                .OrderBy(a => a.ScheduledAt)
                .Take(5)
                .Select(a => new
                {
                    a.Id,
                    a.ProviderId,
                    a.ScheduledAt,
                    Status = a.Status.ToString(),
                    a.ReasonForVisit,
                })
                .ToListAsync(ct);

            return appointments;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load upcoming appointments for patient {PatientId}", patientId);
            return null;
        }
    }

    public async Task<object?> GetRecentActivityAsync(
        Guid patientId, CancellationToken ct = default)
    {
        try
        {
            var recentAppointments = await _db.Appointments
                .AsNoTracking()
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.ScheduledAt)
                .Take(10)
                .Select(a => new
                {
                    Type = "appointment",
                    Description = $"Appointment {a.Status}",
                    Timestamp = a.ScheduledAt,
                    a.Id,
                })
                .ToListAsync(ct);

            return recentAppointments;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load recent activity for patient {PatientId}", patientId);
            return null;
        }
    }
}
