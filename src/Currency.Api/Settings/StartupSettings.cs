using Currency.Infrastructure.Settings;

namespace Currency.Api.Settings;

public class StartupSettings
{
    public string DataProtectionKeysDirectory { get; set; }
    public RateLimiterSettings RateLimiter { get; init; }
    public JwtSettings Jwt { get; init; }
    public IntegrationsSettings Integrations { get; set; }
    public LoggerSettings LoggerSettings { get; init; }
}