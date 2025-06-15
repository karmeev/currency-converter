using System.Text.Json;
using Currency.Infrastructure.Contracts.Databases.Base;
using Currency.Infrastructure.Contracts.Databases.Redis;
using Currency.Infrastructure.Contracts.Databases.Redis.Entries;
using Currency.Infrastructure.Settings;
using StackExchange.Redis;

namespace Currency.Infrastructure.Redis;

internal class RedisContext(
    InfrastructureSettings settings,
    IConnectionMultiplexer connection) : IRedisContext
{
    private const string IndexesSecretKey = "indexes";
    
    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null)
    {
        var serialized = JsonSerializer.Serialize(value);
        var db = GetDatabase(key);
        await db.StringSetAsync(key, serialized, ttl);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var db = GetDatabase(key);
        var value = await db.StringGetAsync(key);
        return value.HasValue ? JsonSerializer.Deserialize<T>(value!) : default;
    }
    
    public async Task<T> GetByIndexAsync<T>(string index)
    {
        var db = GetDatabase(IndexesSecretKey);
        var value = await db.StringGetAsync(index);
        var key = value.ToString();
        db = GetDatabase(index);
        var entity = await db.StringGetAsync(key);
        return JsonSerializer.Deserialize<T>(entity!);
    }
    
    public async Task SortedSetAddAsync(string key, IEnumerable<RedisSortedSetEntry> entries, TimeSpan? ttl = null)
    {
        var db = GetDatabase(key);

        var redisEntries = entries
            .Select(e => new SortedSetEntry(e.Value, e.Score))
            .ToArray();

        await db.SortedSetAddAsync(key, redisEntries);
        
        if (ttl.HasValue)
            await db.KeyExpireAsync(key, ttl);
    }

    public async Task<string[]> SortedSetRangeByRankAsync(string key, long start, long stop, bool ascending = true)
    {
        var db = GetDatabase(key);
        var order = ascending ? Order.Ascending : Order.Descending;

        var redisValues = await db.SortedSetRangeByRankAsync(key, start, stop, order);

        return redisValues.Select(rv => rv.ToString()).ToArray();
    }

    private IDatabase GetDatabase(string key)
    {
        var redisSettings = settings.RedisSettings;
        
        if (key == IndexesSecretKey)
            return connection.GetDatabase(0);
        
        //TODO: add from settings
        if (key.StartsWith(EntityPrefix.AuthPrefix)) 
            return connection.GetDatabase(redisSettings.RefreshTokensDatabaseNumber);
        
        if (key.StartsWith(EntityPrefix.RatesHistoryPrefix)) 
            return connection.GetDatabase(2);
        
        if (key.StartsWith(EntityPrefix.ExchangeRatesPrefix)) 
            return connection.GetDatabase(3);
        
        if (key.StartsWith(EntityPrefix.UserPrefix)) 
            return connection.GetDatabase(15);

        return connection.GetDatabase(redisSettings.EntitiesDatabaseNumber);
    }
}