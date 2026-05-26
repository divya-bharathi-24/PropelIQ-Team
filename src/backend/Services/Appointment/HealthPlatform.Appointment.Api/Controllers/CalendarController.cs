using HealthPlatform.Appointment.Api.Data;
using HealthPlatform.Appointment.Api.Models;
using HealthPlatform.Appointment.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthPlatform.Appointment.Api.Controllers;

[ApiController]
[Route("api/calendar")]
[Authorize(Roles = "Patient,Staff,Provider,Admin")]
public sealed class CalendarController : ControllerBase
{
    private readonly CalendarSyncService _syncService;
    private readonly IcsCalendarService _icsService;
    private readonly AppointmentDbContext _db;

    public CalendarController(
        CalendarSyncService syncService,
        IcsCalendarService icsService,
        AppointmentDbContext db)
    {
        _syncService = syncService;
        _icsService = icsService;
        _db = db;
    }

    [HttpPost("enable-ics")]
    public async Task<IActionResult> EnableIcsFeed(CancellationToken ct)
    {
        var patientId = GetPatientIdFromClaims();
        var connection = await _syncService.EnableIcsFeedAsync(patientId, ct);
        var feedUrl = $"{Request.Scheme}://{Request.Host}/api/calendar/feed/{connection.IcsFeedToken}";

        return Ok(new CalendarSyncResponse("active", feedUrl, null, "ICS feed enabled."));
    }

    [HttpGet("feed/{feedToken}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetIcsFeed(string feedToken, CancellationToken ct)
    {
        var connection = await _db.CalendarConnections
            .FirstOrDefaultAsync(c => c.IcsFeedToken == feedToken && c.Status == "active", ct);

        if (connection is null)
            return NotFound(new { message = "Feed not found or has been invalidated." });

        var icsContent = await _icsService.GenerateIcsFeedAsync(connection.PatientId, ct);
        return Content(icsContent, "text/calendar");
    }

    [HttpGet("google/auth")]
    public async Task<IActionResult> GetGoogleAuthUrl(CancellationToken ct)
    {
        var patientId = GetPatientIdFromClaims();
        var url = await _syncService.GetGoogleAuthUrlAsync(patientId, ct);

        return Ok(new { authUrl = url });
    }

    [HttpGet("google/callback")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleCallback(
        [FromQuery] string code, [FromQuery] string state, CancellationToken ct)
    {
        await _syncService.HandleOAuthCallbackAsync(code, state, ct);

        // Redirect back to the frontend calendar settings page
        return Redirect("/patient/profile?tab=calendar&connected=google");
    }

    [HttpPost("disconnect/{provider}")]
    public async Task<IActionResult> Disconnect(string provider, CancellationToken ct)
    {
        var patientId = GetPatientIdFromClaims();
        var (success, message) = await _syncService.DisconnectAsync(patientId, provider, ct);

        if (!success)
            return BadRequest(new { message });

        return Ok(new CalendarDisconnectResponse(message));
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetSyncStatus(CancellationToken ct)
    {
        var patientId = GetPatientIdFromClaims();
        var connections = await _syncService.GetAllConnectionsAsync(patientId, ct);

        var result = connections.Select(c => new
        {
            c.Provider,
            c.Status,
            icsFeedUrl = c.IcsFeedToken is not null
                ? $"{Request.Scheme}://{Request.Host}/api/calendar/feed/{c.IcsFeedToken}"
                : null,
        });

        return Ok(result);
    }

    private Guid GetPatientIdFromClaims()
    {
        var claim = User.FindFirst("sub")?.Value ?? User.FindFirst("nameid")?.Value;
        return claim is not null ? Guid.Parse(claim) : Guid.Empty;
    }
}
