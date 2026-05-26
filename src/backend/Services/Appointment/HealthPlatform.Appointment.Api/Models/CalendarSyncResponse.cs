namespace HealthPlatform.Appointment.Api.Models;

public sealed record CalendarSyncResponse(
    string Status,
    string? IcsFeedUrl,
    string? GoogleCalendarStatus,
    string? Message);

public sealed record CalendarDisconnectResponse(
    string Message);
