using SendGrid;
using SendGrid.Helpers.Mail;

namespace HealthPlatform.Auth.Api.Services;

public sealed class EmailVerificationService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailVerificationService> _logger;

    public EmailVerificationService(IConfiguration config, ILogger<EmailVerificationService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendVerificationEmailAsync(string email, string token, CancellationToken ct = default)
    {
        var apiKey = _config["SendGrid:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogWarning("SendGrid API key not configured; skipping email send");
            return;
        }

        var baseUrl = _config["App:BaseUrl"] ?? "https://localhost:4200";
        var verificationLink = $"{baseUrl}/auth/activate?token={Uri.EscapeDataString(token)}";

        var client = new SendGridClient(apiKey);
        var from = new EmailAddress(
            _config["SendGrid:FromEmail"] ?? "noreply@healthplatform.com",
            _config["SendGrid:FromName"] ?? "Health Platform");
        var to = new EmailAddress(email);
        var subject = "Verify your email address";
        var htmlContent = $"""
            <h2>Welcome to Health Platform</h2>
            <p>Please click the link below to verify your email address:</p>
            <p><a href="{verificationLink}">Verify Email</a></p>
            <p>This link expires in 24 hours.</p>
            """;

        var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
        var response = await client.SendEmailAsync(msg, ct);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to send verification email to {Email}. Status: {Status}",
                email, response.StatusCode);
        }
    }
}
