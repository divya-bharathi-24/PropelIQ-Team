namespace HealthPlatform.Appointment.Api.Models;

public sealed record AppointmentListResponse(
    IReadOnlyList<AppointmentListItem> Items,
    int TotalCount,
    int Page,
    int PageSize);

public sealed record AppointmentListItem(
    Guid Id,
    Guid ProviderId,
    DateTime ScheduledAt,
    string Status,
    string? ReasonForVisit,
    int RescheduleCount);
