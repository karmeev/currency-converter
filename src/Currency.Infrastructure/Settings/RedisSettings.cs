namespace Currency.Infrastructure.Settings;

public class RedisSettings
{
    public string ConnectionString { get; init; }
    public int DataLockMilliseconds { get; init; } = 100;
    public int DataLockRetryCount  { get; init; } = 2;
    public int DataLockRetryDelayMilliseconds  { get; init; } = 10;
    public int RefreshTokensDatabaseNumber { get; init; } = 1;
    public int ExchangeRatesHistoryDatabaseNumber { get; init; } = 1;
    public int ExchangeRatesDatabaseNumber { get; init; } = 1;
}