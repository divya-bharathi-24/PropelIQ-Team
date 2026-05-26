using HealthPlatform.Auth.Api.Data;
using HealthPlatform.Auth.Api.Models;
using HealthPlatform.Auth.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthPlatform.Auth.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly AuthDbContext _db;
    private readonly ITokenService _tokenService;
    private readonly SessionService _sessionService;
    private readonly RateLimitService _rateLimitService;
    private readonly AuthAuditService _auditService;

    public AuthController(
        AuthDbContext db,
        ITokenService tokenService,
        SessionService sessionService,
        RateLimitService rateLimitService,
        AuthAuditService auditService)
    {
        _db = db;
        _tokenService = tokenService;
        _sessionService = sessionService;
        _rateLimitService = rateLimitService;
        _auditService = auditService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request, CancellationToken ct)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var userAgent = Request.Headers.UserAgent.ToString();

        var (isLocked, retryAfter) = await _rateLimitService.CheckLockoutAsync(request.Email, ct);
        if (isLocked)
        {
            Response.Headers["Retry-After"] = ((int)(retryAfter!.Value - DateTime.UtcNow).TotalSeconds).ToString();
            await _auditService.LogEventAsync("login", request.Email, null, ip, userAgent, false, "account_locked", ct);
            return StatusCode(429, new { message = "Account temporarily locked due to too many failed attempts.", retryAfter });
        }

        var user = await _db.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant(), ct);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            await _auditService.LogEventAsync("login", request.Email, user?.Id, ip, userAgent, false, "invalid_credentials", ct);
            return Unauthorized(new { message = "Invalid email or password." });
        }

        if (user.Status != "active")
        {
            await _auditService.LogEventAsync("login", request.Email, user.Id, ip, userAgent, false, "account_not_active", ct);
            return Unauthorized(new { message = "Account is not active. Please verify your email." });
        }

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToArray();
        var tokens = _tokenService.GenerateTokens(user.Id, user.Email, roles);

        await _sessionService.CreateSessionAsync(user.Id, tokens.RefreshToken, ct);

        user.LastLoginAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        await _auditService.LogEventAsync("login", user.Email, user.Id, ip, userAgent, true, ct: ct);

        if (user.ForcePasswordChange)
        {
            return Ok(new TokenResponse(
                tokens.AccessToken, tokens.RefreshToken,
                tokens.ExpiresAt, roles, ForcePasswordChange: true));
        }

        return Ok(tokens);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var userAgent = Request.Headers.UserAgent.ToString();

        var (valid, userId, email, _) = _tokenService.ValidateAccessToken(request.AccessToken);
        if (!valid)
            return Unauthorized(new { message = "Invalid access token." });

        var newTokens = _tokenService.GenerateTokens(userId, email, []);

        var (success, oldToken) = await _sessionService.RotateRefreshTokenAsync(
            request.RefreshToken, newTokens.RefreshToken, ct);

        if (!success)
        {
            await _auditService.LogEventAsync("refresh_token_reuse", email, userId, ip, userAgent, false, "replay_attack_detected", ct);
            return Unauthorized(new { message = "Invalid refresh token. Please log in again." });
        }

        var user = await _db.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstAsync(u => u.Id == userId, ct);

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToArray();
        var finalTokens = _tokenService.GenerateTokens(userId, email, roles);

        await _sessionService.CreateSessionAsync(userId, finalTokens.RefreshToken, ct);

        await _auditService.LogEventAsync("token_refresh", email, userId, ip, userAgent, true, ct: ct);

        return Ok(finalTokens);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var userAgent = Request.Headers.UserAgent.ToString();

        await _sessionService.RevokeAllUserTokensAsync(userId.Value, ct);
        await _auditService.LogEventAsync("logout", "", userId.Value, ip, userAgent, true, ct: ct);

        return Ok(new { message = "Logged out successfully." });
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var user = await _db.Users.FindAsync([userId.Value], ct);
        if (user is null) return NotFound();

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            return BadRequest(new { message = "Current password is incorrect." });

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, workFactor: 12);
        user.ForcePasswordChange = false;
        await _db.SaveChangesAsync(ct);

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        await _auditService.LogEventAsync("password_change", user.Email, user.Id, ip, null, true, ct: ct);

        return Ok(new { message = "Password changed successfully." });
    }

    private Guid? GetCurrentUserId()
    {
        var sub = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        return Guid.TryParse(sub, out var id) ? id : null;
    }
}

public sealed record RefreshTokenRequest(string AccessToken, string RefreshToken);
public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);
