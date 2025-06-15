using Currency.Data.Contracts;
using Currency.Data.Contracts.Exceptions;
using Currency.Data.Locks;
using Currency.Domain.Operations;
using Currency.Domain.Rates;
using Currency.Infrastructure.Contracts.Databases.Base;
using Currency.Infrastructure.Contracts.Databases.Redis;
using Microsoft.Extensions.Logging;

namespace Currency.Data.Repositories;

internal class ExchangeRatesRepository(
    ILogger<ExchangeRatesRepository> logger,
    IRedisContext context) : IExchangeRatesRepository
{
    private static string Prefix => EntityPrefix.ExchangeRatesPrefix;

    public async Task AddExchangeRates(string id, ExchangeRates exchangeRates, CancellationToken token)
    {
        if (token.IsCancellationRequested) return;
        
        var key = $"{Prefix}:{id}";
        
        try
        {
            await using var @lock = new DataLock((IRedisLockContext)context);
            await @lock.AcquireLockAsync(key);
            
            if (await context.TryGetAsync<ExchangeRates>(key) is not null)
                ConcurrencyException.ThrowIfExists("Exchange Rates already exists", key);

            //TODO: TTL should be implements from settings!
            var ttl = new TimeSpan(0, 0, 3, 0, 0);
            await context.SetAsync(key, exchangeRates, ttl);
        }
        catch (ConcurrencyException ex)
        {
            logger.LogError(ex, "Pessimistic Concurrency: {message}, Key: {key}", ex.Message, ex.LockId);
        }
    }

    public async Task<ExchangeRates> GetExchangeRates(string id, CancellationToken token)
    {
        //if (token.IsCancellationRequested) return;
        
        var rates = await context.TryGetAsync<ExchangeRates>($"{Prefix}:{id}");
        return rates;
    }

    public async Task AddConversionResultAsync(string id, CurrencyConversion conversion, CancellationToken token)
    {
        if (token.IsCancellationRequested) return;

        var key = $"{Prefix}:{id}";
        
        try
        {
            await using var @lock = new DataLock((IRedisLockContext)context);
            await @lock.AcquireLockAsync(key);
            
            if (await context.TryGetAsync<ExchangeRates>(key) is not null)
                ConcurrencyException.ThrowIfExists("Currency conversion already exists", key);

            //TODO: TTL should be implements from settings!
            var ttl = new TimeSpan(0, 0, 3, 0, 0);
            await context.SetAsync(key, conversion, ttl);
        }
        catch (ConcurrencyException ex)
        {
            logger.LogError(ex, "Pessimistic Concurrency: {message}, Key: {key}", ex.Message, ex.LockId);
        }
    }

    public async Task<CurrencyConversion> GetCurrencyConversionAsync(string id, CancellationToken token)
    {
        //if (token.IsCancellationRequested) return;
        
        var currency = await context.TryGetAsync<CurrencyConversion>($"{Prefix}:{id}");
        return currency;
    }
}