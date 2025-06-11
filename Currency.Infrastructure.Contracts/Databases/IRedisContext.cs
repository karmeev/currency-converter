namespace Currency.Infrastructure.Contracts.Databases;

public interface IRedisContext
{
    Task<T?> GetAsync<T>(string key, string dbKey = null);
    Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, string dbKey = null);
}