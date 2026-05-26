using HealthPlatform.Auth.Api.Models;

namespace HealthPlatform.Auth.Api.Services;

public interface ITokenService
{
    TokenResponse GenerateTokens(Guid userId, string email, string[] roles);
    (bool Valid, Guid UserId, string Email, string[] Roles) ValidateAccessToken(string token);
}
