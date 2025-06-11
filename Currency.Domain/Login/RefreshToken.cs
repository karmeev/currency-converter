namespace Currency.Domain.Login;

public struct RefreshToken
{
    public bool Verified { get; init; }
    public string UserId { get; init; }
    public string Token { get; init; }
    public TimeSpan ExpiresAt { get; init; }
    public DateTime ExpirationDate { get; set; }
}