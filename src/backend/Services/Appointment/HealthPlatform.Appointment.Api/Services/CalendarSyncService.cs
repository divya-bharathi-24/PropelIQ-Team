using System.Security.Cryptography;
using HealthPlatform.Appointment.Api.Data;
using HealthPlatform.Appointment.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthPlatform.Appointment.Api.Services;

public sealed class CalendarSyncService
{
    private readonly AppointmentDbContext _db;
    private readonly IConfiguration _configuration;

    public CalendarSyncService(AppointmentDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task<CalendarConnection> EnableIcsFeedAsync(Guid patientId, CancellationToken ct)
    {
        var existing = await _db.CalendarConnections
            .FirstOrDefaultAsync(c => c.PatientId == patientId && c.Provider == "ics", ct);

        if (existing is not null)
            return existing;

        var feedToken = GenerateFeedToken();

        var connection = new CalendarConnection
        {
            PatientId = patientId,
            Provider = "ics",
            IcsFeedToken = feedToken,
            Status = "active",
        };

        _db.CalendarConnections.Add(connection);
        await _db.SaveChangesAsync(ct);

        return connection;
    }

    public Task<string> GetGoogleAuthUrlAsync(Guid patientId, CancellationToken ct)
    {
        var clientId = _configuration["Google:ClientId"] ?? "placeholder-client-id";
        var redirectUri = _configuration["Google:RedirectUri"] ?? "https://localhost:5001/api/calendar/google/callback";

        var state = Convert.ToBase64String(patientId.ToByteArray());

        return Task.FromResult($"https://accounts.google.com/o/oauth2/v2/auth?" +
               $"client_id={Uri.EscapeDataString(clientId)}" +
               $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
               $"&response_type=code" +
               $"&scope={Uri.EscapeDataString("https://www.googleapis.com/auth/calendar")}" +
               $"&access_type=offline" +
               $"&prompt=consent" +
               $"&state={Uri.EscapeDataString(state)}");
    }

    public async Task<CalendarConnection> HandleOAuthCallbackAsync(
        string code, string state, CancellationToken ct)
    {
        var patientId = new Guid(Convert.FromBase64String(state));

        var existing = await _db.CalendarConnections
            .FirstOrDefaultAsync(c => c.PatientId == patientId && c.Provider == "google", ct);

        if (existing is not null)
        {
            existing.Status = "active";
            existing.UpdatedAt = DateTime.UtcNow;
            // In production: exchange code for tokens via Google OAuth
            existing.OAuthAccessToken = $"access_token_for_{code}";
            existing.OAuthRefreshToken = $"refresh_token_for_{code}";
            existing.OAuthTokenExpiresAt = DateTime.UtcNow.AddHours(1);
        }
        else
        {
            existing = new CalendarConnection
            {
                PatientId = patientId,
                Provider = "google",
                OAuthAccessToken = $"access_token_for_{code}",
                OAuthRefreshToken = $"refresh_token_for_{code}",
                OAuthTokenExpiresAt = DateTime.UtcNow.AddHours(1),
                GoogleCalendarId = "Healthcare",
                Status = "active",
            };
            _db.CalendarConnections.Add(existing);
        }

        await _db.SaveChangesAsync(ct);
        return existing;
    }

    public async Task<(bool Success, string Message)> DisconnectAsync(
        Guid patientId, string provider, CancellationToken ct)
    {
        var connection = await _db.CalendarConnections
            .FirstOrDefaultAsync(c => c.PatientId == patientId && c.Provider == provider, ct);

        if (connection is null)
            return (false, "No connection found.");

        if (provider == "google")
        {
            // In production: revoke OAuth token with Google API
            connection.OAuthAccessToken = null;
            connection.OAuthRefreshToken = null;
        }

        if (provider == "ics")
        {
            connection.IcsFeedToken = null;
        }

        connection.Status = "disconnected";
        connection.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        return (true, "Calendar disconnected. Existing entries in your external calendar will remain but no longer update.");
    }

    public async Task<CalendarConnection?> GetConnectionAsync(
        Guid patientId, string provider, CancellationToken ct)
    {
        return await _db.CalendarConnections
            .FirstOrDefaultAsync(c => c.PatientId == patientId && c.Provider == provider, ct);
    }

    public async Task<List<CalendarConnection>> GetAllConnectionsAsync(Guid patientId, CancellationToken ct)
    {
        return await _db.CalendarConnections
            .Where(c => c.PatientId == patientId)
            .ToListAsync(ct);
    }

    private static string GenerateFeedToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }
}
