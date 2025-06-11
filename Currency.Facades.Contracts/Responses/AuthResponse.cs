using System.Security.Claims;

namespace Currency.Facades.Contracts.Responses;

public class AuthResponse
{
    private AuthResponse()
    {
    }

    public AuthResponse(IEnumerable<Claim> claims, string accessToken, string refreshToken, DateTime expiresAt)
    {
        Success = true;
        ErrorMessage = string.Empty;
        Claims = claims;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        ExpiresAt = expiresAt;
    }

    public bool Success { get; private init; }
    public string ErrorMessage { get; private init; }
    public IEnumerable<Claim> Claims { get; init; }
    public string AccessToken { get; init; }
    public string RefreshToken { get; init; }
    public DateTime ExpiresAt { get; init; }

    public static AuthResponse Error(string errorMessage)
    {
        return new AuthResponse { Success = false, ErrorMessage = errorMessage ?? "Unexpected error" };
    }
}