using HealthPlatform.Auth.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace HealthPlatform.Auth.Api.Services;

public sealed class RateLimitService
{
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan AttemptWindow = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(30);

    private readonly AuthDbContext _db;

    public RateLimitService(AuthDbContext db)
    {
        _db = db;
    }

    public async Task<(bool IsLocked, DateTime? RetryAfter)> CheckLockoutAsync(
        string email, CancellationToken ct = default)
    {
        var windowStart = DateTime.UtcNow.Subtract(AttemptWindow);

        var recentFailedAttempts = await _db.LoginAttempts
            .CountAsync(a =>
                a.Email == email.ToLowerInvariant() &&
                !a.Successful &&
                a.AttemptedAt >= windowStart, ct);

        if (recentFailedAttempts < MaxFailedAttempts)
            return (false, null);

        var lastFailedAttempt = await _db.LoginAttempts
            .Where(a =>
                a.Email == email.ToLowerInvariant() &&
                !a.Successful &&
                a.AttemptedAt >= windowStart)
            .OrderByDescending(a => a.AttemptedAt)
            .Select(a => a.AttemptedAt)
            .FirstOrDefaultAsync(ct);

        var lockoutEnd = lastFailedAttempt.Add(LockoutDuration);
        if (lockoutEnd > DateTime.UtcNow)
            return (true, lockoutEnd);

        return (false, null);
    }
}
