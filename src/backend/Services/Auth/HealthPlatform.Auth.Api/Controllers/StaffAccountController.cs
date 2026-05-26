using HealthPlatform.Auth.Api.Models;
using HealthPlatform.Auth.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthPlatform.Auth.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Staff,Provider,Admin")]
public sealed class StaffAccountController : ControllerBase
{
    private readonly StaffAccountService _staffAccountService;

    public StaffAccountController(StaffAccountService staffAccountService)
    {
        _staffAccountService = staffAccountService;
    }

    [HttpPost("create-patient")]
    public async Task<IActionResult> CreatePatient(
        [FromBody] StaffCreatePatientRequest request, CancellationToken ct)
    {
        var staffUserId = GetCurrentUserId();
        if (staffUserId is null) return Unauthorized();

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var result = await _staffAccountService.CreatePatientAccountAsync(
            request.FirstName, request.LastName, request.Phone,
            request.DateOfBirth, staffUserId.Value, ip, ct);

        if (result is null)
            return BadRequest(new { message = "Failed to create patient account." });

        return Ok(new StaffCreatePatientResponse(
            result.Value.PatientId,
            result.Value.TempPassword,
            "Patient account created successfully."));
    }

    private Guid? GetCurrentUserId()
    {
        var sub = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        return Guid.TryParse(sub, out var id) ? id : null;
    }
}
