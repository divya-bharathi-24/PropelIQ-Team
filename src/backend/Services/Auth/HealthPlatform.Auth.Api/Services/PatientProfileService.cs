using HealthPlatform.Auth.Api.Data;
using HealthPlatform.Auth.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthPlatform.Auth.Api.Services;

public sealed class PatientProfileService
{
    private readonly AuthDbContext _db;

    public PatientProfileService(AuthDbContext db)
    {
        _db = db;
    }

    public async Task<PatientProfileDto?> GetProfileAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null) return null;

        return new PatientProfileDto(
            user.Id, user.FirstName, user.LastName,
            user.Email, user.Phone, user.DateOfBirth,
            user.ProfilePhotoPath);
    }

    public async Task<(bool Success, string Message)> UpdateProfileAsync(
        Guid userId, UpdateProfileRequest request, CancellationToken ct = default)
    {
        var user = await _db.Users.FindAsync([userId], ct);
        if (user is null)
            return (false, "User not found.");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Phone = request.Phone;
        user.DateOfBirth = request.DateOfBirth;

        await _db.SaveChangesAsync(ct);
        return (true, "Profile updated successfully.");
    }
}
