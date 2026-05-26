using HealthPlatform.Appointment.Api.Models;
using HealthPlatform.Appointment.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthPlatform.Appointment.Api.Controllers;

[ApiController]
[Route("api/swap")]
[Authorize(Roles = "Patient,Staff,Provider,Admin")]
public sealed class SwapController : ControllerBase
{
    private readonly SwapService _swapService;
    private readonly SwapQueueService _queueService;

    public SwapController(SwapService swapService, SwapQueueService queueService)
    {
        _swapService = swapService;
        _queueService = queueService;
    }

    [HttpPost("request")]
    public async Task<IActionResult> CreateSwapRequest(
        [FromBody] SwapRequestDto request, CancellationToken ct)
    {
        var patientId = GetPatientIdFromClaims();
        var (success, message, swapRequestId, queuePosition) = await _swapService.CreateSwapRequestAsync(
            patientId, request.OriginalSlotId, request.ProviderId,
            request.PreferredDate, request.PreferredTimeStart, request.PreferredTimeEnd, ct);

        if (!success)
            return BadRequest(new { message });

        return Ok(new SwapResponseDto(swapRequestId!.Value, "pending", queuePosition));
    }

    [HttpPost("accept")]
    public async Task<IActionResult> AcceptSwap(
        [FromBody] SwapAcceptRequest request, CancellationToken ct)
    {
        var patientId = GetPatientIdFromClaims();
        var (success, message) = await _swapService.AcceptSwapAsync(patientId, request.SwapRequestId, ct);

        return success ? Ok(new { message }) : BadRequest(new { message });
    }

    [HttpPost("{swapRequestId:guid}/cancel")]
    public async Task<IActionResult> CancelSwap(Guid swapRequestId, CancellationToken ct)
    {
        var patientId = GetPatientIdFromClaims();
        var (success, message) = await _swapService.CancelSwapAsync(patientId, swapRequestId, ct);

        return success ? Ok(new { message }) : BadRequest(new { message });
    }

    [HttpGet("{swapRequestId:guid}/status")]
    public async Task<IActionResult> GetSwapStatus(Guid swapRequestId, CancellationToken ct)
    {
        var position = await _queueService.GetQueuePositionAsync(swapRequestId, ct);
        return Ok(new { swapRequestId, queuePosition = position });
    }

    private Guid GetPatientIdFromClaims()
    {
        var claim = User.FindFirst("sub")?.Value ?? User.FindFirst("nameid")?.Value;
        return claim is not null ? Guid.Parse(claim) : Guid.Empty;
    }
}
