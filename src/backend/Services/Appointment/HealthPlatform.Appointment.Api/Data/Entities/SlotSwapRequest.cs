using System.ComponentModel.DataAnnotations;

namespace HealthPlatform.Appointment.Api.Data.Entities;

public enum SwapRequestStatus
{
    Pending,
    Matched,
    Accepted,
    Rejected,
    Expired
}

public sealed class SlotSwapRequest
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid OriginalSlotId { get; set; }
    public Guid? MatchedSlotId { get; set; }
    public Guid ProviderId { get; set; }

    public DateOnly PreferredDate { get; set; }
    public TimeOnly? PreferredTimeStart { get; set; }
    public TimeOnly? PreferredTimeEnd { get; set; }

    public SwapRequestStatus Status { get; set; } = SwapRequestStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
}
