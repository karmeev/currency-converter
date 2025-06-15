namespace Currency.Domain.Login;

public struct AccessToken
{
    public string Token { get; set; }
    public DateTime ExpiresAt { get; init; }
}