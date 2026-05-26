using System.ComponentModel.DataAnnotations;

namespace HealthPlatform.Appointment.Api.Models;

public sealed record InsuranceCheckRequest(
    [Required, MaxLength(200)] string InsuranceProvider,
    [Required, MaxLength(50)] string PolicyNumber,
    [MaxLength(50)] string? GroupNumber,
    [Required, MaxLength(50)] string MemberId,
    Guid PatientId);

public sealed record InsuranceCheckResponse(
    string CoverageStatus,
    decimal? CopayEstimate,
    string? Limitations,
    DateTime VerifiedAt);
