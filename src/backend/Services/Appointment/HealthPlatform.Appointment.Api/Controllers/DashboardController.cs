using HealthPlatform.Appointment.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthPlatform.Appointment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Patient,Staff,Provider,Admin")]
public sealed class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboardService;

    public DashboardController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("{patientId:guid}")]
    public async Task<IActionResult> GetDashboard(
        Guid patientId, CancellationToken ct)
    {
        var upcomingAppointments = await _dashboardService.GetUpcomingAppointmentsAsync(patientId, ct);
        var recentActivity = await _dashboardService.GetRecentActivityAsync(patientId, ct);

        var quickActions = new[]
        {
            new { Key = "book_appointment", Label = "Book Appointment", Available = true },
            new { Key = "upload_document", Label = "Upload Document", Available = true },
            new { Key = "view_medical_history", Label = "View Medical History", Available = true },
            new { Key = "contact_support", Label = "Contact Support", Available = true },
        };

        return Ok(new
        {
            upcomingAppointments,
            recentActivity,
            quickActions,
        });
    }
}
