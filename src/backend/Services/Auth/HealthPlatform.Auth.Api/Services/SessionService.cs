using HealthPlatform.Auth.Api.Data;
using HealthPlatform.Auth.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthPlatform.Auth.Api.Services;

public sealed class SessionService
{
    private const int MaxConcurrentSessions = 3;
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

    private readonly AuthDbContext _db;
    private readonly ILogger<SessionService> _logger;

    public SessionService(AuthDbContext db, ILogger<SessionService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<RefreshToken> CreateSessionAsync(
        Guid userId, string refreshTokenValue, CancellationToken ct = default)
    {
        var activeSessions = await _db.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null && t.ExpiresAt > DateTime.UtcNow)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync(ct);

        if (activeSessions.Count >= MaxConcurrentSessions)
        {
            var oldest = activeSessions.First();
            oldest.RevokedAt = DateTime.UtcNow;
            _logger.LogInformation(
                "Session limit reached for user {UserId}. Terminated oldest session.",
                userId);
        }

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = refreshTokenValue,
            ExpiresAt = DateTime.UtcNow.Add(RefreshTokenLifetime),
        };

        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync(ct);

        return refreshToken;
    }

    public async Task<(bool Success, RefreshToken? OldToken)> RotateRefreshTokenAsync(
        string currentToken, string newTokenValue, CancellationToken ct = default)
    {
        var existingToken = await _db.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == currentToken, ct);

        if (existingToken is null)
            return (false, null);

        if (existingToken.IsRevoked)
        {
            _logger.LogWarning(
                "Refresh token reuse detected for user {UserId}. Revoking ALL tokens.",
                existingToken.UserId);

            await RevokeAllUserTokensAsync(existingToken.UserId, ct);
            return (false, null);
        }

        if (existingToken.IsExpired)
            return (false, null);

        existingToken.RevokedAt = DateTime.UtcNow;
        existingToken.ReplacedByToken = newTokenValue;

        var newToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = existingToken.UserId,
            Token = newTokenValue,
            ExpiresAt = DateTime.UtcNow.Add(RefreshTokenLifetime),
        };

        _db.RefreshTokens.Add(newToken);
        await _db.SaveChangesAsync(ct);

        return (true, existingToken);
    }

    public async Task RevokeAllUserTokensAsync(Guid userId, CancellationToken ct = default)
    {
        var activeTokens = await _db.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync(ct);

        foreach (var token in activeTokens)
            token.RevokedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
    }
}
