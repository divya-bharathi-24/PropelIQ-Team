using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace HealthPlatform.Shared.Caching;

/// <summary>
/// Redis cache-aside implementation with TTL, jitter for stampede prevention,
/// and fallback to direct DB query on connection failure.
/// </summary>
public sealed class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisCacheService> _logger;
    private static readonly Random Jitter = new();

    public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class
    {
        try
        {
            var db = _redis.GetDatabase();
            var value = await db.StringGetAsync(key);
            if (value.IsNullOrEmpty)
                return null;

            return JsonSerializer.Deserialize<T>(value!);
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogWarning(ex, "Redis connection failure on GET {Key}; falling back to DB", key);
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default) where T : class
    {
        try
        {
            var db = _redis.GetDatabase();
            var json = JsonSerializer.Serialize(value);
            await db.StringSetAsync(key, json, ttl);
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogWarning(ex, "Redis connection failure on SET {Key}; skipping cache write", key);
        }
    }

    public async Task RemoveAsync(IEnumerable<string> keys, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var redisKeys = keys.Select(k => (RedisKey)k).ToArray();
            await db.KeyDeleteAsync(redisKeys);
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogWarning(ex, "Redis connection failure on DELETE; cache may be stale");
        }
    }

    public async Task<T> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan ttl,
        CancellationToken ct = default) where T : class
    {
        // Probabilistic early expiration (jitter) to prevent thundering herd
        var jitteredTtl = ApplyJitter(ttl);

        var cached = await GetAsync<T>(key, ct);
        if (cached is not null)
            return cached;

        // Cache miss or Redis failure — load from source
        var value = await factory(ct);
        await SetAsync(key, value, jitteredTtl, ct);
        return value;
    }

    /// <summary>Apply ±10% jitter to TTL to prevent cache stampede.</summary>
    private static TimeSpan ApplyJitter(TimeSpan baseTtl)
    {
        var jitterFactor = 0.9 + (Jitter.NextDouble() * 0.2); // 0.9 to 1.1
        return TimeSpan.FromMilliseconds(baseTtl.TotalMilliseconds * jitterFactor);
    }
}
