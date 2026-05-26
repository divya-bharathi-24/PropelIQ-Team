namespace HealthPlatform.Shared.Caching;

/// <summary>
/// Centralized cache key generation and invalidation logic.
/// All cache keys are prefixed by domain to prevent collisions.
/// </summary>
public static class CacheKeyManager
{
    // Schedule TTL: 5 minutes
    public static readonly TimeSpan ScheduleTtl = TimeSpan.FromMinutes(5);

    // Availability TTL: 30 seconds
    public static readonly TimeSpan AvailabilityTtl = TimeSpan.FromSeconds(30);

    public static string ProviderSchedule(Guid providerId, DateOnly date)
        => $"schedule:{providerId}:{date:yyyy-MM-dd}";

    public static string SlotAvailability(Guid providerId, DateOnly date)
        => $"availability:{providerId}:{date:yyyy-MM-dd}";

    public static string PatientAppointments(Guid patientId)
        => $"patient-appointments:{patientId}";

    /// <summary>
    /// Returns all cache keys that must be invalidated when an appointment
    /// is booked or cancelled for the given provider and date.
    /// </summary>
    public static IEnumerable<string> GetInvalidationKeys(Guid providerId, Guid patientId, DateOnly date)
    {
        yield return ProviderSchedule(providerId, date);
        yield return SlotAvailability(providerId, date);
        yield return PatientAppointments(patientId);
    }
}
