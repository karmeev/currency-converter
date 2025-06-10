namespace Currency.Api.Settings;

public record AuthSettings(
    string Secret,
    int TokenExpirationMinutes,
    string Issuer,
    string Audience);