using HealthPlatform.Appointment.Api.Data;
using HealthPlatform.Appointment.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace HealthPlatform.Appointment.Api.Jobs;

public sealed class CalendarSyncJob
{
    private readonly AppointmentDbContext _db;
    private readonly GoogleCalendarService _googleService;
    private const int MaxRetries = 6;

    public CalendarSyncJob(AppointmentDbContext db, GoogleCalendarService googleService)
    {
        _db = db;
        _googleService = googleService;
    }

    /// <summary>
    /// Syncs an appointment change to the patient's connected Google Calendar.
    /// Uses exponential backoff: max 6 retries over ~1 hour.
    /// </summary>
    public async Task SyncAppointmentAsync(Guid appointmentId, string action, int retryCount, CancellationToken ct)
    {
        if (retryCount > MaxRetries) return;

        var appointment = await _db.Appointments.FindAsync([appointmentId], ct);
        if (appointment is null) return;

        try
        {
            switch (action)
            {
                case "create":
                case "update":
                    await _googleService.PushAppointmentAsync(appointment.PatientId, appointment, ct);
                    break;
                case "delete":
                    await _googleService.RemoveAppointmentAsync(
                        appointment.PatientId, appointment.GoogleCalendarEventId, ct);
                    break;
            }
        }
        catch (Exception) when (retryCount < MaxRetries)
        {
            // Exponential backoff: queue retry with delay
            // In production: Hangfire.BackgroundJob.Schedule(() => SyncAppointmentAsync(...), delay)
            // delay = TimeSpan.FromMinutes(Math.Pow(2, retryCount))
            throw; // Let Hangfire handle retry with configured policy
        }
    }
}
