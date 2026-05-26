namespace HealthPlatform.Notification.Api.Services;

public sealed class AppointmentEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AppointmentEmailService> _logger;

    public AppointmentEmailService(IConfiguration configuration, ILogger<AppointmentEmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Sends appointment confirmation email with ICS attachment via SendGrid.
    /// Target: within 60 seconds of booking/reschedule.
    /// </summary>
    public async Task SendConfirmationEmailAsync(
        string patientEmail, string patientName,
        Guid appointmentId, DateTime scheduledAt, string? reason, string icsContent)
    {
        var apiKey = _configuration["SendGrid:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogWarning("SendGrid API key not configured. Skipping email for appointment {AppointmentId}.", appointmentId);
            return;
        }

        var client = new SendGrid.SendGridClient(apiKey);
        var from = new SendGrid.Helpers.Mail.EmailAddress(
            _configuration["SendGrid:FromEmail"] ?? "noreply@healthplatform.com", "Health Platform");
        var to = new SendGrid.Helpers.Mail.EmailAddress(patientEmail, patientName);

        var msg = SendGrid.Helpers.Mail.MailHelper.CreateSingleEmail(
            from, to,
            "Appointment Confirmation",
            $"Your appointment on {scheduledAt:MMMM dd, yyyy} at {scheduledAt:h:mm tt} UTC has been confirmed.",
            $"<h2>Appointment Confirmed</h2>" +
            $"<p>Date: {scheduledAt:MMMM dd, yyyy}</p>" +
            $"<p>Time: {scheduledAt:h:mm tt} UTC</p>" +
            $"<p>Reason: {reason ?? "General consultation"}</p>" +
            $"<p>Appointment ID: {appointmentId}</p>");

        // Attach ICS calendar file
        var icsBytes = System.Text.Encoding.UTF8.GetBytes(icsContent);
        msg.AddAttachment("appointment.ics", Convert.ToBase64String(icsBytes), "text/calendar");

        var response = await client.SendEmailAsync(msg);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to send appointment email. Status: {StatusCode}", response.StatusCode);
        }
    }
}
