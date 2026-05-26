using HealthPlatform.Appointment.Api.Data;
using HealthPlatform.Appointment.Api.Models;
using HealthPlatform.Appointment.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthPlatform.Appointment.Api.Controllers;

[ApiController]
[Route("api/appointments")]
[Authorize(Roles = "Patient,Staff,Provider,Admin")]
public sealed class AppointmentController : ControllerBase
{
    private readonly AppointmentManagementService _managementService;
    private readonly PdfConfirmationService _pdfService;
    private readonly IcsCalendarService _icsService;
    private readonly AppointmentDbContext _db;

    public AppointmentController(
        AppointmentManagementService managementService,
        PdfConfirmationService pdfService,
        IcsCalendarService icsService,
        AppointmentDbContext db)
    {
        _managementService = managementService;
        _pdfService = pdfService;
        _icsService = icsService;
        _db = db;
    }

    [HttpGet("{patientId:guid}")]
    public async Task<IActionResult> GetAppointments(
        Guid patientId,
        [FromQuery] int page = 1,
        [FromQuery] string? status = null,
        [FromQuery] string sortBy = "date",
        [FromQuery] bool descending = false,
        CancellationToken ct = default)
    {
        var result = await _managementService.GetAppointmentsAsync(patientId, page, status, sortBy, descending, ct);
        return Ok(result);
    }

    [HttpPost("reschedule")]
    public async Task<IActionResult> Reschedule(
        [FromBody] RescheduleRequest request, CancellationToken ct)
    {
        // PatientId would come from JWT claims in production
        var patientId = GetPatientIdFromClaims();
        var (success, message) = await _managementService.RescheduleAsync(patientId, request, ct);

        return success ? Ok(new { message }) : BadRequest(new { message });
    }

    [HttpPost("{appointmentId:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid appointmentId,
        [FromBody] CancelRequest? request,
        CancellationToken ct)
    {
        var patientId = GetPatientIdFromClaims();
        var (success, message) = await _managementService.CancelAsync(patientId, appointmentId, request?.Reason, ct);

        return success ? Ok(new { message }) : BadRequest(new { message });
    }

    [HttpGet("{appointmentId:guid}/pdf")]
    public async Task<IActionResult> DownloadPdf(Guid appointmentId, CancellationToken ct)
    {
        var appointment = await GetAppointmentAsync(appointmentId, ct);
        if (appointment is null) return NotFound();

        try
        {
            var pdf = _pdfService.GenerateConfirmationPdf(
                appointment.Id, appointment.ScheduledAt, appointment.ReasonForVisit, appointment.ProviderId);
            return File(pdf, "application/pdf", $"confirmation-{appointmentId}.pdf");
        }
        catch
        {
            return StatusCode(503, new { message = "PDF will be available shortly." });
        }
    }

    [HttpGet("{appointmentId:guid}/ics")]
    public async Task<IActionResult> DownloadIcs(Guid appointmentId, CancellationToken ct)
    {
        var appointment = await GetAppointmentAsync(appointmentId, ct);
        if (appointment is null) return NotFound();

        var ics = _icsService.GenerateIcsForAppointment(appointment);
        return File(System.Text.Encoding.UTF8.GetBytes(ics), "text/calendar", "appointment.ics");
    }

    private Guid GetPatientIdFromClaims()
    {
        var claim = User.FindFirst("sub")?.Value ?? User.FindFirst("nameid")?.Value;
        return claim is not null ? Guid.Parse(claim) : Guid.Empty;
    }

    private async Task<Data.Entities.Appointment?> GetAppointmentAsync(Guid appointmentId, CancellationToken ct)
    {
        return await _db.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId, ct);
    }

    public sealed record CancelRequest(string? Reason);
}
