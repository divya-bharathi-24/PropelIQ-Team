using HealthPlatform.Appointment.Api.Models;
using HealthPlatform.Appointment.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthPlatform.Appointment.Api.Controllers;

[ApiController]
[Route("api/insurance")]
[Authorize(Roles = "Patient,Staff,Provider,Admin")]
public sealed class InsuranceController : ControllerBase
{
    private readonly InsuranceCheckService _insuranceService;

    public InsuranceController(InsuranceCheckService insuranceService)
    {
        _insuranceService = insuranceService;
    }

    [HttpPost("check")]
    public async Task<IActionResult> CheckEligibility(
        [FromBody] InsuranceCheckRequest request,
        CancellationToken ct)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(TimeSpan.FromSeconds(5));

        try
        {
            var result = await _insuranceService.CheckEligibilityAsync(request, cts.Token);
            return Ok(result);
        }
        catch (OperationCanceledException)
        {
            // Timeout: return pending status, Hangfire will retry
            return Ok(new InsuranceCheckResponse(
                "Verification Pending", null, null, DateTime.UtcNow));
        }
    }
}
