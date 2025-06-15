using Microsoft.Extensions.Options;

namespace Currency.Infrastructure.Settings;

public class InfrastructureSettings(
    IOptionsMonitor<JwtSettings> jwt,
    IOptionsMonitor<RedisSettings> redis,
    IOptionsMonitor<FrankfurterSettings> frank)
{
    public JwtSettings JwtSettings => jwt.CurrentValue;
    public RedisSettings RedisSettings => redis.CurrentValue;
    public IntegrationsSettings Integrations => new()
    {
        Frankfurter = frank.CurrentValue
    };
}
