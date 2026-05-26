using HealthPlatform.Auth.Api.Data;
using HealthPlatform.Auth.Api.Data.Entities;

namespace HealthPlatform.Auth.Api.Services;

public sealed class AuthAuditService
{
    private readonly AuthDbContext _db;
    private readonly ILogger<AuthAuditService> _logger;

    public AuthAuditService(AuthDbContext db, ILogger<AuthAuditService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task LogEventAsync(
        string eventType, string email, Guid? userId,
        string ipAddress, string? userAgent,
        bool successful, string? failureReason = null,
        CancellationToken ct = default)
    {
        var attempt = new LoginAttempt
        {
            Id = Guid.NewGuid(),
            EventType = eventType,
            Email = email,
            UserId = userId,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Successful = successful,
            FailureReason = failureReason,
        };

        _db.LoginAttempts.Add(attempt);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Auth event: {EventType} for {Email} - Success: {Success}",
            eventType, email, successful);
    }
}
