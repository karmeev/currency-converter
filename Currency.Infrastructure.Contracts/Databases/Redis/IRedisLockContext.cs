namespace Currency.Infrastructure.Contracts.Databases.Redis;

public interface IRedisLockContext : IRedisContext
{
    Task<bool> AcquireLockAsync(string key, string lockId);
    Task<bool> ReleaseLockAsync(string key, string lockId);
}