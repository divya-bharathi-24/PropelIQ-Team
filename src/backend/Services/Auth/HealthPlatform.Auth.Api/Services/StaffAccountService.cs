using System.Security.Cryptography;
using HealthPlatform.Auth.Api.Data;
using HealthPlatform.Auth.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthPlatform.Auth.Api.Services;

public sealed class StaffAccountService
{
    private readonly AuthDbContext _db;
    private readonly AuthAuditService _auditService;
    private readonly ILogger<StaffAccountService> _logger;

    public StaffAccountService(
        AuthDbContext db,
        AuthAuditService auditService,
        ILogger<StaffAccountService> logger)
    {
        _db = db;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<(Guid PatientId, string TempPassword)?> CreatePatientAccountAsync(
        string firstName, string lastName, string phone, DateOnly dateOfBirth,
        Guid staffUserId, string staffIp, CancellationToken ct = default)
    {
        var tempPassword = GenerateTemporaryPassword();
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword, workFactor: 12);
        var email = $"{phone.Replace("+", "").Replace(" ", "")}@temp.healthplatform.local";

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            Phone = phone,
            DateOfBirth = dateOfBirth,
            Status = "active",
            IsActive = true,
            ForcePasswordChange = true,
            CreatedByStaffId = staffUserId,
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

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            _logger.LogWarning("Concurrency conflict creating patient. Retrying.");
            _db.ChangeTracker.Clear();
            user.Id = Guid.NewGuid();
            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);
        }

        await _auditService.LogEventAsync(
            "staff_create_patient", email, user.Id, staffIp, null, true,
            $"Created by staff {staffUserId}", ct);

        _logger.LogInformation(
            "Staff {StaffId} created patient account {PatientId}",
            staffUserId, user.Id);

        return (user.Id, tempPassword);
    }

    private static string GenerateTemporaryPassword()
    {
        const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string lower = "abcdefghjkmnpqrstuvwxyz";
        const string digits = "23456789";
        const string special = "!@#$%&*";

        var password = new char[12];
        password[0] = upper[RandomNumberGenerator.GetInt32(upper.Length)];
        password[1] = lower[RandomNumberGenerator.GetInt32(lower.Length)];
        password[2] = digits[RandomNumberGenerator.GetInt32(digits.Length)];
        password[3] = special[RandomNumberGenerator.GetInt32(special.Length)];

        var allChars = upper + lower + digits + special;
        for (int i = 4; i < 12; i++)
            password[i] = allChars[RandomNumberGenerator.GetInt32(allChars.Length)];

        RandomNumberGenerator.Shuffle<char>(password);
        return new string(password);
    }
}
