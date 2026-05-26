namespace HealthPlatform.Notification.Api.Services;

public sealed class SwapNotificationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SwapNotificationService> _logger;

    public SwapNotificationService(IConfiguration configuration, ILogger<SwapNotificationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Sends swap availability notification via push + email.
    /// Target: within 60 seconds of slot becoming available.
    /// </summary>
    public async Task NotifySwapAvailableAsync(
        string patientEmail, string patientName, Guid swapRequestId, DateTime slotTime)
    {
        var apiKey = _configuration["SendGrid:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogWarning("SendGrid API key not configured. Skipping swap notification for {SwapRequestId}.", swapRequestId);
            return;
        }

        var client = new SendGrid.SendGridClient(apiKey);
        var from = new SendGrid.Helpers.Mail.EmailAddress(
            _configuration["SendGrid:FromEmail"] ?? "noreply@healthplatform.com", "Health Platform");
        var to = new SendGrid.Helpers.Mail.EmailAddress(patientEmail, patientName);

        var msg = SendGrid.Helpers.Mail.MailHelper.CreateSingleEmail(
            from, to,
            "Preferred Slot Available - Action Required",
            $"Your preferred appointment slot on {slotTime:MMMM dd, yyyy} at {slotTime:h:mm tt} UTC is now available. " +
            $"You have 15 minutes to accept this swap.",
            $"<h2>Preferred Slot Available!</h2>" +
            $"<p>Your preferred slot on <strong>{slotTime:MMMM dd, yyyy} at {slotTime:h:mm tt} UTC</strong> is now available.</p>" +
            $"<p><strong>You have 15 minutes to accept this swap.</strong></p>" +
            $"<p>Log in to your account to accept or decline.</p>");

        var response = await client.SendEmailAsync(msg);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to send swap notification email. Status: {StatusCode}", response.StatusCode);
        }
    }
}
