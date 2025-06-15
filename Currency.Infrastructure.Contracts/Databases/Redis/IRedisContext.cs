using Currency.Infrastructure.Contracts.Databases.Redis.Entries;

namespace Currency.Infrastructure.Contracts.Databases.Redis;

public interface IRedisContext
{
    Task<T?> GetAsync<T>(string key);
    Task<T> GetByIndexAsync<T>(string index);
    Task SetAsync<T>(string key, T value, TimeSpan? ttl = null);
    Task SortedSetAddAsync(string key, IEnumerable<RedisSortedSetEntry> entries, TimeSpan? ttl = null);
    Task<string[]> SortedSetRangeByRankAsync(string key, long start, long stop, bool ascending = true);
}