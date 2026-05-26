using System.ComponentModel.DataAnnotations;

namespace HealthPlatform.Appointment.Api.Data.Entities;

public sealed class ProviderSchedule
{
    public Guid Id { get; set; }
    public Guid ProviderId { get; set; }

    public DateOnly ScheduleDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public int SlotDurationMinutes { get; set; } = 30;
    public bool IsActive { get; set; } = true;

    public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
}
