using HealthPlatform.Auth.Api.Data;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace HealthPlatform.Auth.Api.Services;

public sealed class PhotoUploadService
{
    private const int MaxFileSizeBytes = 2 * 1024 * 1024; // 2MB
    private const int TargetSize = 200;
    private static readonly HashSet<string> AllowedContentTypes = ["image/jpeg", "image/png"];

    private readonly AuthDbContext _db;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<PhotoUploadService> _logger;

    public PhotoUploadService(
        AuthDbContext db,
        IWebHostEnvironment env,
        ILogger<PhotoUploadService> logger)
    {
        _db = db;
        _env = env;
        _logger = logger;
    }

    public async Task<(bool Success, string Message)> UploadPhotoAsync(
        Guid userId, IFormFile file, CancellationToken ct = default)
    {
        if (file.Length > MaxFileSizeBytes)
            return (false, "File exceeds maximum size of 2MB.");

        if (!AllowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
            return (false, "Only JPEG and PNG files are allowed.");

        var user = await _db.Users.FindAsync([userId], ct);
        if (user is null)
            return (false, "User not found.");

        var uploadsDir = Path.Combine(_env.ContentRootPath, "uploads", "photos");
        Directory.CreateDirectory(uploadsDir);

        var extension = file.ContentType == "image/png" ? ".png" : ".jpg";
        var fileName = $"{userId}{extension}";
        var filePath = Path.Combine(uploadsDir, fileName);

        using var stream = file.OpenReadStream();
        using var image = await Image.LoadAsync(stream, ct);
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(TargetSize, TargetSize),
            Mode = ResizeMode.Crop,
        }));

        await image.SaveAsync(filePath, ct);

        user.ProfilePhotoPath = $"/uploads/photos/{fileName}";
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Profile photo uploaded for user {UserId}", userId);
        return (true, user.ProfilePhotoPath);
    }
}
