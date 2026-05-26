using HealthPlatform.Appointment.Api.Data;
using HealthPlatform.Appointment.Api.Data.Entities;
using HealthPlatform.Appointment.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace HealthPlatform.Appointment.Api.Jobs;

public sealed class SlotMonitorJob
{
    private readonly AppointmentDbContext _db;
    private readonly SwapQueueService _queueService;

    public SlotMonitorJob(AppointmentDbContext db, SwapQueueService queueService)
    {
        _db = db;
        _queueService = queueService;
    }

    /// <summary>
    /// Checks for newly available slots that match pending swap requests.
    /// Called on slot cancellation/reschedule events or as a recurring check.
    /// </summary>
    public async Task ExecuteAsync(Guid slotId, CancellationToken ct)
    {
        var slot = await _db.TimeSlots.FindAsync([slotId], ct);
        if (slot is null || !slot.IsAvailable) return;

        var slotDate = DateOnly.FromDateTime(slot.StartTime);

        var nextRequest = await _queueService.GetNextInQueueAsync(slot.ProviderId, slotDate, ct);
        if (nextRequest is null) return;

        await _queueService.MatchRequestAsync(nextRequest.Id, slotId, ct);
    }
}
