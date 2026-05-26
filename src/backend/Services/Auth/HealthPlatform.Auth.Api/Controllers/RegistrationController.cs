using HealthPlatform.Auth.Api.Models;
using HealthPlatform.Auth.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HealthPlatform.Auth.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class RegistrationController : ControllerBase
{
    private readonly RegistrationService _registrationService;

    public RegistrationController(RegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request, CancellationToken ct)
    {
        var (success, message) = await _registrationService.RegisterAsync(
            request.FirstName, request.LastName, request.Email,
            request.Phone, request.DateOfBirth, request.Password, ct);

        if (!success)
            return BadRequest(new RegisterResponse(message));

        return Ok(new RegisterResponse(message));
    }

    [HttpGet("activate")]
    public async Task<IActionResult> Activate(
        [FromQuery] string token, CancellationToken ct)
    {
        var (success, message) = await _registrationService.ActivateAccountAsync(token, ct);

        if (!success)
            return BadRequest(new { message });

        return Ok(new { message });
    }

    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification(
        [FromBody] ResendVerificationRequest request, CancellationToken ct)
    {
        var (success, message) = await _registrationService.ResendVerificationAsync(
            request.Email, ct);

        if (!success)
            return StatusCode(429, new { message });

        return Ok(new { message });
    }
}

public sealed record ResendVerificationRequest(string Email);
