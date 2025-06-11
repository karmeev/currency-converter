namespace Currency.Data.Models;

public class RefreshTokenModel
{
    public bool Verified { get; init; }
    public string UserId { get; init; }
    public string Token { get; init; }
    public TimeSpan ExpiresAt { get; init; }
    public DateTime ExpirationDate { get; set; }
}