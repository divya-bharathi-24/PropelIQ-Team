namespace HealthPlatform.Appointment.Api.Data.Entities;

public sealed class TimeSlot
{
    public Guid Id { get; set; }
    public Guid ProviderId { get; set; }
    public Guid ScheduleId { get; set; }
    public ProviderSchedule Schedule { get; set; } = null!;

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsAvailable { get; set; } = true;
    public bool IsBlocked { get; set; }
}
