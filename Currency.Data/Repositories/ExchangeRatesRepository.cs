using Currency.Data.Contracts;
using Currency.Data.Contracts.Exceptions;
using Currency.Data.Locks;
using Currency.Data.Settings;
using Currency.Domain.Operations;
using Currency.Domain.Rates;
using Currency.Infrastructure.Contracts.Databases.Base;
using Currency.Infrastructure.Contracts.Databases.Redis;
using Microsoft.Extensions.Logging;

namespace Currency.Data.Repositories;

internal class ExchangeRatesRepository(
    ILogger<ExchangeRatesRepository> logger,
    DataSettings settings,
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

            await context.SetAsync(key, exchangeRates, settings.ExchangeRatesTtl);
        }
        catch (ConcurrencyException ex)
        {
            logger.LogError(ex, "Pessimistic Concurrency: {message}, Key: {key}", ex.Message, ex.LockId);
        }
    }

    public async Task<ExchangeRates> GetExchangeRates(string id, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
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
            
            await context.SetAsync(key, conversion, settings.ConversionResultTtl);
        }
        catch (ConcurrencyException ex)
        {
            logger.LogError(ex, "Pessimistic Concurrency: {message}, Key: {key}", ex.Message, ex.LockId);
        }
    }

    public async Task<CurrencyConversion> GetCurrencyConversionAsync(string id, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        var currency = await context.TryGetAsync<CurrencyConversion>($"{Prefix}:{id}");
        return currency;
    }
}