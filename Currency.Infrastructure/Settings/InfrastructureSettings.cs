namespace Currency.Infrastructure.Settings;

public class InfrastructureSettings
{
    public JwtSettings JwtSettings { get; set; }
    public RedisSettings RedisSettings { get; set; }
    public IntegrationsSettings Integrations { get; set; }
}