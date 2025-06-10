using System.Security.Claims;

namespace Currency.Facades.Contracts.Responses;

public class LoginResponse
{
    public bool Success { get; private init; }
    public string ErrorMessage { get; private init; }
    public IEnumerable<Claim> Claims { get; init; }
    public string AccessToken { get; init; }
    public string RefreshToken { get; init; }

    private LoginResponse() { }
    
    public LoginResponse(IEnumerable<Claim> claims, string accessToken, string refreshToken)
    {
        Success = true;
        ErrorMessage = string.Empty;
        Claims = claims;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public static LoginResponse Error(string errorMessage)
    {
        return new LoginResponse { Success = false, ErrorMessage = errorMessage ?? "Unexpected error" };
    }
}