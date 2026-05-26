namespace HealthPlatform.Auth.Api.Models;

public sealed record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    DateOnly DateOfBirth,
    string Password
);
