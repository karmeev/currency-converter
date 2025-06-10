namespace Currency.Api.Settings;

public record ApiSettings(
    AuthSettings AuthSettings, 
    RateLimiterSettings RateLimiterSettings);