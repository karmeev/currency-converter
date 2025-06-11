using Currency.Infrastructure.Settings;

namespace Currency.Api.Settings;

public record ApiSettings(
    RateLimiterSettings RateLimiterSettings,
    InfrastructureSettings InfrastructureSettings);