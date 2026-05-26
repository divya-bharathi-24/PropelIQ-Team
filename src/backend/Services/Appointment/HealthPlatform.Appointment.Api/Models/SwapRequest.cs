using System.ComponentModel.DataAnnotations;

namespace HealthPlatform.Appointment.Api.Models;

public sealed record SwapRequestDto(
    [Required] Guid OriginalSlotId,
    [Required] Guid ProviderId,
    [Required] DateOnly PreferredDate,
    TimeOnly? PreferredTimeStart,
    TimeOnly? PreferredTimeEnd);

public sealed record SwapAcceptRequest(
    [Required] Guid SwapRequestId);

public sealed record SwapResponseDto(
    Guid SwapRequestId,
    string Status,
    int? QueuePosition);
