namespace Currency.Infrastructure.Settings;

public record JwtSettings
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int AccessTokenTtlInMinutes { get; set; }
    public int RefreshTokenTtlInDays { get; set; }
    public string SecurityKey { get; set; }
}