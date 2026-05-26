using HealthPlatform.Appointment.Api.Data;
using HealthPlatform.Appointment.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthPlatform.Appointment.Api.Services;

public sealed class SwapQueueService
{
    private readonly AppointmentDbContext _db;

    public SwapQueueService(AppointmentDbContext db)
    {
        _db = db;
    }

    public async Task<int> GetQueuePositionAsync(Guid swapRequestId, CancellationToken ct)
    {
        var request = await _db.SlotSwapRequests.FindAsync([swapRequestId], ct);
        if (request is null) return 0;

        // FIFO queue: count pending requests for same provider+date created before this one
        var position = await _db.SlotSwapRequests.CountAsync(
            r => r.ProviderId == request.ProviderId &&
                 r.PreferredDate == request.PreferredDate &&
                 r.Status == SwapRequestStatus.Pending &&
                 r.CreatedAt <= request.CreatedAt,
            ct);

        return position;
    }

    public async Task<SlotSwapRequest?> GetNextInQueueAsync(
        Guid providerId, DateOnly preferredDate, CancellationToken ct)
    {
        return await _db.SlotSwapRequests
            .Where(r => r.ProviderId == providerId &&
                        r.PreferredDate == preferredDate &&
                        r.Status == SwapRequestStatus.Pending)
            .OrderBy(r => r.CreatedAt)
            .FirstOrDefaultAsync(ct);
    }

    public async Task MatchRequestAsync(Guid swapRequestId, Guid slotId, CancellationToken ct)
    {
        var request = await _db.SlotSwapRequests.FindAsync([swapRequestId], ct);
        if (request is null) return;

        request.MatchedSlotId = slotId;
        request.Status = SwapRequestStatus.Matched;
        request.ExpiresAt = DateTime.UtcNow.AddMinutes(15);

        await _db.SaveChangesAsync(ct);
    }

    public async Task ExpireAndAdvanceAsync(Guid swapRequestId, CancellationToken ct)
    {
        var request = await _db.SlotSwapRequests.FindAsync([swapRequestId], ct);
        if (request is null || request.Status != SwapRequestStatus.Matched) return;

        request.Status = SwapRequestStatus.Expired;
        await _db.SaveChangesAsync(ct);

        // Advance to next in queue
        if (request.MatchedSlotId.HasValue)
        {
            var next = await GetNextInQueueAsync(request.ProviderId, request.PreferredDate, ct);
            if (next is not null)
            {
                await MatchRequestAsync(next.Id, request.MatchedSlotId.Value, ct);
            }
        }
    }
}
