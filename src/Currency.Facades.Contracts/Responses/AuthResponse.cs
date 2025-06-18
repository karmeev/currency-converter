namespace Currency.Facades.Contracts.Responses;

public struct AuthResponse
{
    public bool Succeeded { get; private init; }
    public string ErrorMessage { get; private init; }
    public string AccessToken { get; private init; }
    public string RefreshToken { get; private init; }
    public DateTime ExpiresAt { get; private init; }

    public static AuthResponse Success(string accessToken, string refreshToken, DateTime expiresAt)
    {
        return new AuthResponse
        {
            Succeeded = true, 
            ErrorMessage = string.Empty,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt
        };
    }
    
    public static AuthResponse Error(string errorMessage)
    {
        return new AuthResponse { Succeeded = false, ErrorMessage = errorMessage ?? "Unexpected error" };
    }
}