using System.Security.Cryptography;
using HealthPlatform.Auth.Api.Data;
using HealthPlatform.Auth.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthPlatform.Auth.Api.Services;

public sealed class RegistrationService
{
    private readonly AuthDbContext _db;
    private readonly EmailVerificationService _emailService;
    private readonly ILogger<RegistrationService> _logger;

    public RegistrationService(
        AuthDbContext db,
        EmailVerificationService emailService,
        ILogger<RegistrationService> logger)
    {
        _db = db;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<(bool Success, string Message)> RegisterAsync(
        string firstName, string lastName, string email, string phone,
        DateOnly dateOfBirth, string password, CancellationToken ct = default)
    {
        var existingUser = await _db.Users
            .AnyAsync(u => u.Email == email.ToLowerInvariant(), ct);

        if (existingUser)
        {
            _logger.LogWarning("Registration attempt with existing email");
            return (false, "Registration could not be completed. Please try again.");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        var verificationToken = GenerateSecureToken();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            Phone = phone,
            DateOfBirth = dateOfBirth,
            Status = "pending_verification",
            IsActive = false,
            VerificationToken = verificationToken,
            VerificationTokenExpiresAt = DateTime.UtcNow.AddHours(24),
        };

        _db.Users.Add(user);

        var patientRole = await _db.Roles
            .FirstOrDefaultAsync(r => r.Name == "Patient", ct);

        if (patientRole is not null)
        {
            _db.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = patientRole.Id,
            });
        }

        await _db.SaveChangesAsync(ct);

        await _emailService.SendVerificationEmailAsync(user.Email, verificationToken, ct);

        _logger.LogInformation("User registered successfully: {UserId}", user.Id);
        return (true, "Registration successful. Please check your email to verify your account.");
    }

    public async Task<(bool Success, string Message)> ActivateAccountAsync(
        string token, CancellationToken ct = default)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u =>
                u.VerificationToken == token &&
                u.Status == "pending_verification", ct);

        if (user is null)
            return (false, "Invalid or expired verification link.");

        if (user.VerificationTokenExpiresAt < DateTime.UtcNow)
            return (false, "Verification link has expired. Please request a new one.");

        user.Status = "active";
        user.IsActive = true;
        user.VerificationToken = null;
        user.VerificationTokenExpiresAt = null;

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Account activated: {UserId}", user.Id);
        return (true, "Account activated successfully. You can now log in.");
    }

    public async Task<(bool Success, string Message)> ResendVerificationAsync(
        string email, CancellationToken ct = default)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u =>
                u.Email == email.ToLowerInvariant() &&
                u.Status == "pending_verification", ct);

        if (user is null)
            return (true, "If the email exists, a verification email has been sent.");

        if (user.LastVerificationEmailSentAt.HasValue &&
            (DateTime.UtcNow - user.LastVerificationEmailSentAt.Value).TotalSeconds < 60)
        {
            return (false, "Please wait before requesting another verification email.");
        }

        var oneHourAgo = DateTime.UtcNow.AddHours(-1);
        if (user.LastVerificationEmailSentAt < oneHourAgo)
            user.VerificationEmailsSentInLastHour = 0;

        if (user.VerificationEmailsSentInLastHour >= 3)
            return (false, "Maximum verification emails reached. Please try again later.");

        var newToken = GenerateSecureToken();
        user.VerificationToken = newToken;
        user.VerificationTokenExpiresAt = DateTime.UtcNow.AddHours(24);
        user.VerificationEmailsSentInLastHour++;
        user.LastVerificationEmailSentAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        await _emailService.SendVerificationEmailAsync(user.Email, newToken, ct);

        return (true, "If the email exists, a verification email has been sent.");
    }

    private static string GenerateSecureToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }
}
