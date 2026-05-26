using System.ComponentModel.DataAnnotations;

namespace HealthPlatform.Appointment.Api.Data.Entities;

public sealed class CalendarConnection
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }

    [MaxLength(20)]
    public string Provider { get; set; } = "google"; // "google" or "ics"

    [MaxLength(2048)]
    public string? OAuthAccessToken { get; set; }

    [MaxLength(2048)]
    public string? OAuthRefreshToken { get; set; }

    public DateTime? OAuthTokenExpiresAt { get; set; }

    [MaxLength(256)]
    public string? IcsFeedToken { get; set; }

    [MaxLength(100)]
    public string? GoogleCalendarId { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "active"; // active, disconnected, delayed

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
