namespace Currency.Domain.Login;

public struct Tokens
{
    public string AccessToken { get; init; }
    public string RefreshToken { get; init; }
    public DateTime ExpiresAt { get; init; }
}