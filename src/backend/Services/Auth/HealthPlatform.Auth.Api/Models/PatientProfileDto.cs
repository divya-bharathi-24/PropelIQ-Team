namespace HealthPlatform.Auth.Api.Models;

public sealed record PatientProfileDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    DateOnly? DateOfBirth,
    string? ProfilePhotoPath
);

public sealed record UpdateProfileRequest(
    string FirstName,
    string LastName,
    string? Phone,
    DateOnly? DateOfBirth
);
