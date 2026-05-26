namespace HealthPlatform.Shared.Caching;

/// <summary>Cache service interface for DI — abstracts Redis from consuming services.</summary>
public interface ICacheService
{
    /// <summary>Get a cached value by key. Returns null on miss or Redis failure (fallback to DB).</summary>
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class;

    /// <summary>Set a value in cache with the specified TTL.</summary>
    Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default) where T : class;

    /// <summary>Remove one or more cache keys.</summary>
    Task RemoveAsync(IEnumerable<string> keys, CancellationToken ct = default);

    /// <summary>
    /// Cache-aside pattern: get from cache, or load from factory and cache with TTL.
    /// On Redis failure, falls through to factory (direct DB query).
    /// </summary>
    Task<T> GetOrSetAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan ttl, CancellationToken ct = default) where T : class;
}
