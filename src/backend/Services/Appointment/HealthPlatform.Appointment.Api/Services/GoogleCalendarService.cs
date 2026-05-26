using HealthPlatform.Appointment.Api.Data;
using HealthPlatform.Appointment.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthPlatform.Appointment.Api.Services;

public sealed class GoogleCalendarService
{
    private readonly AppointmentDbContext _db;

    public GoogleCalendarService(AppointmentDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Pushes an appointment to the patient's Google Calendar "Healthcare" calendar.
    /// In production, uses Google Calendar API v3.
    /// </summary>
    public async Task PushAppointmentAsync(Guid patientId, Data.Entities.Appointment appointment, CancellationToken ct)
    {
        var connection = await _db.CalendarConnections
            .FirstOrDefaultAsync(c => c.PatientId == patientId && c.Provider == "google" && c.Status == "active", ct);

        if (connection is null) return;

        // Check token expiry and attempt silent refresh
        if (connection.OAuthTokenExpiresAt.HasValue && connection.OAuthTokenExpiresAt.Value < DateTime.UtcNow)
        {
            var refreshed = await TryRefreshTokenAsync(connection, ct);
            if (!refreshed)
            {
                connection.Status = "disconnected";
                await _db.SaveChangesAsync(ct);
                return;
            }
        }

        // In production: use Google.Apis.Calendar.v3 to create/update event
        // For now, store the event ID for tracking
        appointment.GoogleCalendarEventId = $"gcal_{appointment.Id}";
        await _db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Removes an appointment from the patient's Google Calendar.
    /// </summary>
    public async Task RemoveAppointmentAsync(Guid patientId, string? googleEventId, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(googleEventId)) return;

        var connection = await _db.CalendarConnections
            .FirstOrDefaultAsync(c => c.PatientId == patientId && c.Provider == "google" && c.Status == "active", ct);

        if (connection is null) return;

        // In production: use Google.Apis.Calendar.v3 to delete event
    }

    private async Task<bool> TryRefreshTokenAsync(CalendarConnection connection, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(connection.OAuthRefreshToken))
            return false;

        // In production: exchange refresh token for new access token via Google OAuth
        connection.OAuthAccessToken = $"refreshed_token_{DateTime.UtcNow.Ticks}";
        connection.OAuthTokenExpiresAt = DateTime.UtcNow.AddHours(1);
        connection.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return true;
    }
}
