using System.ComponentModel.DataAnnotations;

namespace HealthPlatform.Appointment.Api.Data.Entities;

public enum AppointmentStatus
{
    Scheduled,
    CheckedIn,
    InProgress,
    Completed,
    Cancelled,
    NoShow
}

public sealed class Appointment
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public Guid TimeSlotId { get; set; }
    public TimeSlot TimeSlot { get; set; } = null!;

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

    [MaxLength(500)]
    public string? ReasonForVisit { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public DateTime ScheduledAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int RescheduleCount { get; set; }

    [MaxLength(500)]
    public string? CancellationReason { get; set; }

    [MaxLength(100)]
    public string? GoogleCalendarEventId { get; set; }
}
