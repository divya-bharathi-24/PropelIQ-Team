using HealthPlatform.Appointment.Api.Data;
using HealthPlatform.Appointment.Api.Data.Entities;
using HealthPlatform.Appointment.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthPlatform.Appointment.Api.Services;

public sealed class InsuranceCheckService
{
    private readonly AppointmentDbContext _db;

    // Dummy insurer records for eligibility matching
    private static readonly List<DummyInsurerRecord> DummyRecords =
    [
        new("BlueCross", "Active-Covered", 25.00m, null),
        new("Aetna", "Active-Covered", 30.00m, null),
        new("United", "Active-Partial Coverage", 50.00m, "Specialist visits limited to 12/year"),
        new("Cigna", "Active-Covered", 20.00m, null),
        new("Humana", "Active-Partial Coverage", 40.00m, "Prior authorization required for imaging"),
    ];

    public InsuranceCheckService(AppointmentDbContext db)
    {
        _db = db;
    }

    public async Task<InsuranceCheckResponse> CheckEligibilityAsync(InsuranceCheckRequest request, CancellationToken ct)
    {
        // Match against dummy records
        var match = DummyRecords.FirstOrDefault(r =>
            request.InsuranceProvider.Contains(r.ProviderName, StringComparison.OrdinalIgnoreCase));

        var now = DateTime.UtcNow;

        if (match is null)
        {
            return new InsuranceCheckResponse("Unable to Verify", null, null, now);
        }

        // Save verified insurance details to patient profile
        var existing = await _db.InsuranceDetails
            .FirstOrDefaultAsync(i => i.PatientId == request.PatientId && i.IsPrimary, ct);

        if (existing is not null)
        {
            existing.InsuranceProvider = request.InsuranceProvider;
            existing.PolicyNumber = request.PolicyNumber;
            existing.GroupNumber = request.GroupNumber;
            existing.MemberId = request.MemberId;
            existing.CoverageStatus = match.Status;
            existing.CopayEstimate = match.CopayEstimate;
            existing.Limitations = match.Limitations;
            existing.LastVerifiedAt = now;
        }
        else
        {
            _db.InsuranceDetails.Add(new InsuranceDetails
            {
                PatientId = request.PatientId,
                InsuranceProvider = request.InsuranceProvider,
                PolicyNumber = request.PolicyNumber,
                GroupNumber = request.GroupNumber,
                MemberId = request.MemberId,
                CoverageStatus = match.Status,
                CopayEstimate = match.CopayEstimate,
                Limitations = match.Limitations,
                IsPrimary = true,
                LastVerifiedAt = now,
            });
        }

        await _db.SaveChangesAsync(ct);

        return new InsuranceCheckResponse(match.Status, match.CopayEstimate, match.Limitations, now);
    }

    private sealed record DummyInsurerRecord(string ProviderName, string Status, decimal? CopayEstimate, string? Limitations);
}
