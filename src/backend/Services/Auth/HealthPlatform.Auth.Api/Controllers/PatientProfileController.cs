using HealthPlatform.Auth.Api.Models;
using HealthPlatform.Auth.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthPlatform.Auth.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Patient,Staff,Provider,Admin")]
public sealed class PatientProfileController : ControllerBase
{
    private readonly PatientProfileService _profileService;
    private readonly PhotoUploadService _photoService;

    public PatientProfileController(
        PatientProfileService profileService,
        PhotoUploadService photoService)
    {
        _profileService = profileService;
        _photoService = photoService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var profile = await _profileService.GetProfileAsync(userId.Value, ct);
        if (profile is null) return NotFound();

        return Ok(profile);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateProfileRequest request, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var (success, message) = await _profileService.UpdateProfileAsync(userId.Value, request, ct);
        if (!success) return BadRequest(new { message });

        return Ok(new { message });
    }

    [HttpPost("photo")]
    [RequestSizeLimit(2 * 1024 * 1024)]
    public async Task<IActionResult> UploadPhoto(
        IFormFile file, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        if (file is null || file.Length == 0)
            return BadRequest(new { message = "No file provided." });

        var (success, message) = await _photoService.UploadPhotoAsync(userId.Value, file, ct);
        if (!success)
        {
            return file.Length > 2 * 1024 * 1024
                ? StatusCode(413, new { message })
                : BadRequest(new { message });
        }

        return Ok(new { photoUrl = message });
    }

    private Guid? GetCurrentUserId()
    {
        var sub = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        return Guid.TryParse(sub, out var id) ? id : null;
    }
}
