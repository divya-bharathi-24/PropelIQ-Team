using Microsoft.Extensions.Logging;

namespace HealthPlatform.Shared.Jobs;

/// <summary>
/// Weekly Hangfire job: archives notification logs older than 90 days to cold storage.
/// Scheduled: weekly on Sundays at 04:00 UTC (after retention job).
/// Cron: "0 4 * * 0"
/// </summary>
public sealed class NotificationArchivalJob
{
    private readonly ILogger<NotificationArchivalJob> _logger;

    public NotificationArchivalJob(ILogger<NotificationArchivalJob> logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Starting notification log archival job");

        var archivalThreshold = DateTime.UtcNow.AddDays(-90);

        // Archive notification logs where ArchiveAfter < threshold
        _logger.LogInformation(
            "Archiving notification logs with ArchiveAfter before {Threshold}",
            archivalThreshold);

        // Placeholder: actual implementation will move records from NotificationDbContext
        // to cold storage table/partition
        await Task.CompletedTask;

        _logger.LogInformation("Notification archival job completed");
    }
}
