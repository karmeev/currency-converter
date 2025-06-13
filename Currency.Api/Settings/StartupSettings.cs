using Currency.Infrastructure.Settings;

namespace Currency.Api.Settings;

public class StartupSettings
{
    public RateLimiterSettings RateLimiter { get; init; }
    public JwtSettings Jwt { get; init; }
    public IntegrationsSettings Integrations { get; init; }
}