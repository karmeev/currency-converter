using System.Text.Json;
using Currency.Domain.Login;
using Currency.Infrastructure.Contracts.Databases;
using Currency.Infrastructure.Settings;
using StackExchange.Redis;

namespace Currency.Infrastructure.Redis;

internal class RedisContext(
    RedisSettings settings,
    IConnectionMultiplexer connection): IRedisContext
{
    private const string AuthDb = "auth";
    
    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, string dbKey = null)
    {
        var serialized = JsonSerializer.Serialize(value);
        var db = GetDatabase(dbKey ?? string.Empty);
        await db.StringSetAsync(key, serialized, ttl);
    }
    
    public async Task<T?> GetAsync<T>(string key, string dbKey = null)
    {
        var db = GetDatabase(dbKey ?? string.Empty);
        var value = await db.StringGetAsync(key);
        return value.HasValue ? JsonSerializer.Deserialize<T>(value!) : default;
    }
    
    private IDatabase GetDatabase(string dbKey)
    {
        if (dbKey == AuthDb)
        {
            return connection.GetDatabase(settings.RefreshTokensDatabaseNumber);
        }

        return connection.GetDatabase(settings.EntitiesDatabaseNumber);
    }
}