namespace Currency.Infrastructure.Settings;

public class RedisSettings
{
    public string ConnectionString { get; init; }
    public int RefreshTokensDatabaseNumber { get; init; } = 1;
}