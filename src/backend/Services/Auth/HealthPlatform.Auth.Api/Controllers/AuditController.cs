using HealthPlatform.Auth.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthPlatform.Auth.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public sealed class AuditController : ControllerBase
{
    private readonly AuthDbContext _db;

    public AuditController(AuthDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAuditLog(
        [FromQuery] string? userId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string? eventType,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string sortBy = "attemptedAt",
        [FromQuery] string sortDirection = "desc",
        CancellationToken ct = default)
    {
        if (pageSize > 100) pageSize = 100;
        if (page < 1) page = 1;

        var query = _db.LoginAttempts.AsNoTracking();

        if (Guid.TryParse(userId, out var uid))
            query = query.Where(a => a.UserId == uid);

        if (startDate.HasValue)
            query = query.Where(a => a.AttemptedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(a => a.AttemptedAt <= endDate.Value);

        if (!string.IsNullOrWhiteSpace(eventType))
            query = query.Where(a => a.EventType == eventType);

        query = (sortBy.ToLowerInvariant(), sortDirection.ToLowerInvariant()) switch
        {
            ("email", "asc") => query.OrderBy(a => a.Email),
            ("email", _) => query.OrderByDescending(a => a.Email),
            ("eventtype", "asc") => query.OrderBy(a => a.EventType),
            ("eventtype", _) => query.OrderByDescending(a => a.EventType),
            ("successful", "asc") => query.OrderBy(a => a.Successful),
            ("successful", _) => query.OrderByDescending(a => a.Successful),
            (_, "asc") => query.OrderBy(a => a.AttemptedAt),
            _ => query.OrderByDescending(a => a.AttemptedAt),
        };

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new
            {
                a.Id,
                a.EventType,
                a.Email,
                a.UserId,
                a.IpAddress,
                a.UserAgent,
                a.Successful,
                a.FailureReason,
                a.AttemptedAt,
            })
            .ToListAsync(ct);

        return Ok(new
        {
            items,
            totalCount,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
        });
    }
}
