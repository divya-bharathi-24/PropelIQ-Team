using HealthPlatform.Shared.Caching;
using Microsoft.Extensions.Logging;

namespace HealthPlatform.Appointment.Api.Caching;

/// <summary>
/// Invalidates slot/schedule cache keys when appointment mutations occur.
/// Must be called from appointment booking/cancellation handlers.
/// Ensures next read fetches from PostgreSQL (cache-aside).
/// </summary>
public sealed class SlotCacheInvalidator
{
    private readonly ICacheService _cache;
    private readonly ILogger<SlotCacheInvalidator> _logger;

    public SlotCacheInvalidator(ICacheService cache, ILogger<SlotCacheInvalidator> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Invalidate all cache keys related to a provider's schedule and patient's appointments.
    /// Called within 1 second of appointment booking or cancellation.
    /// </summary>
    public async Task InvalidateOnMutationAsync(
        Guid providerId, Guid patientId, DateOnly appointmentDate, CancellationToken ct = default)
    {
        var keys = CacheKeyManager.GetInvalidationKeys(providerId, patientId, appointmentDate).ToList();

        _logger.LogInformation(
            "Invalidating {Count} cache keys for provider {ProviderId} on {Date}",
            keys.Count, providerId, appointmentDate);

        await _cache.RemoveAsync(keys, ct);
    }
}
