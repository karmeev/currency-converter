using System.Text.Json;
using Currency.Infrastructure.Contracts.Databases;
using Currency.Infrastructure.Settings;
using StackExchange.Redis;

namespace Currency.Infrastructure.Redis;

internal class RedisContext(
    InfrastructureSettings settings,
    IConnectionMultiplexer connection) : IRedisContext
{
    private const string AuthPrefix = "auth";

    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, string dbKey = null)
    {
        var serialized = JsonSerializer.Serialize(value);
        var db = GetDatabase(key);
        await db.StringSetAsync(key, serialized, ttl);
    }

    public async Task<T?> GetAsync<T>(string key, string dbKey = null)
    {
        var db = GetDatabase(key);
        var value = await db.StringGetAsync(key);
        return value.HasValue ? JsonSerializer.Deserialize<T>(value!) : default;
    }

    private IDatabase GetDatabase(string key)
    {
        var redisSettings = settings.RedisSettings;
        if (key.StartsWith(AuthPrefix)) 
            return connection.GetDatabase(redisSettings.RefreshTokensDatabaseNumber);

        return connection.GetDatabase(redisSettings.EntitiesDatabaseNumber);
    }
}