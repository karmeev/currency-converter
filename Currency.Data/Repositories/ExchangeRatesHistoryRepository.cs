using Currency.Data.Contracts;
using Currency.Data.Contracts.Entries;
using Currency.Infrastructure.Contracts.Databases.Base;
using Currency.Infrastructure.Contracts.Databases.Redis;
using Currency.Infrastructure.Contracts.Databases.Redis.Entries;
using Newtonsoft.Json;

namespace Currency.Data.Repositories;

public class ExchangeRatesHistoryRepository(IRedisContext context): IExchangeRatesHistoryRepository
{
    private static string Prefix => EntityPrefix.RatesHistoryPrefix;

    public async Task<IEnumerable<ExchangeRateEntry>> GetRateHistoryPagedAsync(string id, int pageNumber, int pageSize, 
        CancellationToken token)
    {
        if (token.IsCancellationRequested) return [];

        var key = $"{Prefix}:{id}";
        var start = (pageNumber - 1) * pageSize;
        var stop = start + pageSize - 1;

        var rawValues = await context.SortedSetRangeByRankAsync(key, start, stop, ascending: true);
        if (rawValues.Length == 0) return [];

        var result = rawValues
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(json => JsonConvert.DeserializeObject<ExchangeRateEntry>(json)!)
            .ToList();

        return result;
    }
    
    public async Task AddRateHistory(string id, IEnumerable<ExchangeRateEntry> entries, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return;

        var redisEntries = entries.Select(entry =>
        {
            var value = JsonConvert.SerializeObject(entry);
            var score = new DateTimeOffset(entry.Date).ToUnixTimeMilliseconds();
            return new RedisSortedSetEntry(value, score);
        });

        //TODO: TTL should be implements from settings!
        var ttl = new TimeSpan(0,0,1,0,0);
        await context.SortedSetAddAsync($"{Prefix}:{id}", redisEntries, ttl);
    }
}