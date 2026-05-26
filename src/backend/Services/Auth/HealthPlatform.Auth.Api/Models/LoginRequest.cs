namespace HealthPlatform.Auth.Api.Models;

public sealed record LoginRequest(
    string Email,
    string Password,
    bool RememberMe = false
);
