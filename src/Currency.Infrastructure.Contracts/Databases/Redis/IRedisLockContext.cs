namespace Currency.Infrastructure.Contracts.Databases.Redis;

public interface IRedisLockContext : IRedisContext
{
    public int RetryCount { get; }
    public int RetryDelayMilliseconds { get; }
    Task<bool> AcquireLockAsync(string key, string lockId);
    Task<bool> ReleaseLockAsync(string key, string lockId);
}