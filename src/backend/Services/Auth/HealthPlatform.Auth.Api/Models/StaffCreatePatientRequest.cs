namespace HealthPlatform.Auth.Api.Models;

public sealed record StaffCreatePatientRequest(
    string FirstName,
    string LastName,
    string Phone,
    DateOnly DateOfBirth
);

public sealed record StaffCreatePatientResponse(
    Guid PatientId,
    string TemporaryPassword,
    string Message
);
