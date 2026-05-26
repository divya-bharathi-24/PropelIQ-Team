using HealthPlatform.Appointment.Api.Data;
using HealthPlatform.Appointment.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthPlatform.Appointment.Api.Services;

public sealed class SwapService
{
    private readonly AppointmentDbContext _db;
    private readonly SwapQueueService _queueService;
    private const int MaxConcurrentRequests = 3;
    private const int MinLeadTimeHours = 2;

    public SwapService(AppointmentDbContext db, SwapQueueService queueService)
    {
        _db = db;
        _queueService = queueService;
    }

    public async Task<(bool Success, string Message, Guid? SwapRequestId, int? QueuePosition)> CreateSwapRequestAsync(
        Guid patientId, Guid originalSlotId, Guid providerId, DateOnly preferredDate,
        TimeOnly? preferredTimeStart, TimeOnly? preferredTimeEnd, CancellationToken ct)
    {
        // Validate concurrent limit
        var activeCount = await _db.SlotSwapRequests.CountAsync(
            r => r.PatientId == patientId &&
                 (r.Status == SwapRequestStatus.Pending || r.Status == SwapRequestStatus.Matched),
            ct);

        if (activeCount >= MaxConcurrentRequests)
            return (false, $"Maximum {MaxConcurrentRequests} concurrent swap requests allowed.", null, null);

        // Validate lead time
        var preferredSlot = await _db.TimeSlots
            .Where(s => s.ProviderId == providerId &&
                        DateOnly.FromDateTime(s.StartTime) == preferredDate &&
                        (preferredTimeStart == null || TimeOnly.FromDateTime(s.StartTime) >= preferredTimeStart))
            .OrderBy(s => s.StartTime)
            .FirstOrDefaultAsync(ct);

        if (preferredSlot is not null && (preferredSlot.StartTime - DateTime.UtcNow).TotalHours < MinLeadTimeHours)
            return (false, "Swap requests require at least 2 hours lead time.", null, null);

        var swapRequest = new SlotSwapRequest
        {
            PatientId = patientId,
            OriginalSlotId = originalSlotId,
            ProviderId = providerId,
            PreferredDate = preferredDate,
            PreferredTimeStart = preferredTimeStart,
            PreferredTimeEnd = preferredTimeEnd,
            Status = SwapRequestStatus.Pending,
        };

        _db.SlotSwapRequests.Add(swapRequest);
        await _db.SaveChangesAsync(ct);

        var position = await _queueService.GetQueuePositionAsync(swapRequest.Id, ct);

        return (true, "Swap request created.", swapRequest.Id, position);
    }

    public async Task<(bool Success, string Message)> AcceptSwapAsync(
        Guid patientId, Guid swapRequestId, CancellationToken ct)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(ct);

        var request = await _db.SlotSwapRequests
            .FirstOrDefaultAsync(r => r.Id == swapRequestId && r.PatientId == patientId, ct);

        if (request is null)
            return (false, "Swap request not found.");

        if (request.Status != SwapRequestStatus.Matched)
            return (false, "Swap request is not in a matched state.");

        if (request.ExpiresAt.HasValue && request.ExpiresAt.Value < DateTime.UtcNow)
            return (false, "Swap acceptance window has expired.");

        if (request.MatchedSlotId is null)
            return (false, "No matched slot available.");

        // Atomic swap: release old slot, reserve new
        var oldSlot = await _db.TimeSlots.FindAsync([request.OriginalSlotId], ct);
        var newSlot = await _db.TimeSlots.FindAsync([request.MatchedSlotId.Value], ct);

        if (oldSlot is null || newSlot is null)
            return (false, "Slot data unavailable.");

        if (!newSlot.IsAvailable)
            return (false, "Preferred slot is no longer available.");

        // Release old slot
        oldSlot.IsAvailable = true;

        // Reserve new slot
        newSlot.IsAvailable = false;

        // Update appoointment
        var appointment = await _db.Appointments
            .FirstOrDefaultAsync(a => a.PatientId == patientId && a.TimeSlotId == request.OriginalSlotId &&
                                      a.Status == AppointmentStatus.Scheduled, ct);

        if (appointment is not null)
        {
            appointment.TimeSlotId = newSlot.Id;
            appointment.ScheduledAt = newSlot.StartTime;
            appointment.UpdatedAt = DateTime.UtcNow;
        }

        request.Status = SwapRequestStatus.Accepted;

        await _db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return (true, "Swap completed successfully.");
    }

    public async Task<(bool Success, string Message)> CancelSwapAsync(
        Guid patientId, Guid swapRequestId, CancellationToken ct)
    {
        var request = await _db.SlotSwapRequests
            .FirstOrDefaultAsync(r => r.Id == swapRequestId && r.PatientId == patientId, ct);

        if (request is null)
            return (false, "Swap request not found.");

        if (request.Status is SwapRequestStatus.Accepted or SwapRequestStatus.Rejected or SwapRequestStatus.Expired)
            return (false, "Swap request cannot be cancelled in its current state.");

        request.Status = SwapRequestStatus.Rejected;

        await _db.SaveChangesAsync(ct);

        return (true, "Swap request cancelled.");
    }
}
