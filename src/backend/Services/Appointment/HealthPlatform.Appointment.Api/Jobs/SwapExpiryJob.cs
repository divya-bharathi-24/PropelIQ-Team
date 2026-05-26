using HealthPlatform.Appointment.Api.Services;

namespace HealthPlatform.Appointment.Api.Jobs;

public sealed class SwapExpiryJob
{
    private readonly SwapQueueService _queueService;

    public SwapExpiryJob(SwapQueueService queueService)
    {
        _queueService = queueService;
    }

    /// <summary>
    /// Expires a matched swap request after the 15-minute acceptance window
    /// and advances the queue to the next waiting patient.
    /// </summary>
    public async Task ExecuteAsync(Guid swapRequestId, CancellationToken ct)
    {
        await _queueService.ExpireAndAdvanceAsync(swapRequestId, ct);
    }
}
