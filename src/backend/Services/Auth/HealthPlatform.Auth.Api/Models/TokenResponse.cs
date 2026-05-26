namespace HealthPlatform.Auth.Api.Models;

public sealed record TokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    string[] Roles,
    bool ForcePasswordChange = false
);
