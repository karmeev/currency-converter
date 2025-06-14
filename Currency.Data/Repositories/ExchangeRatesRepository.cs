using Currency.Data.Contracts;
using Currency.Domain.Operations;
using Currency.Domain.Rates;
using Currency.Infrastructure.Contracts.Databases.Base;
using Currency.Infrastructure.Contracts.Databases.Redis;

namespace Currency.Data.Repositories;

internal class ExchangeRatesRepository(IRedisContext context) : IExchangeRatesRepository
{
    private static string Prefix => EntityPrefix.ExchangeRatesPrefix;

    public async Task AddExchangeRates(string id, ExchangeRates exchangeRates, CancellationToken token)
    {
        if (token.IsCancellationRequested) return;
        
        //TODO: TTL should be implements from settings!
        var ttl = new TimeSpan(0,0,3,0,0);
        await context.SetAsync($"{Prefix}{id}", exchangeRates, ttl);
    }

    public async Task<ExchangeRates> GetExchangeRates(string id, CancellationToken token)
    {
        //if (token.IsCancellationRequested) return;
        
        var rates = await context.GetAsync<ExchangeRates>($"{Prefix}{id}");
        return rates;
    }

    public async Task AddConversionResultAsync(string id, CurrencyConversion conversion, CancellationToken token)
    {
        if (token.IsCancellationRequested) return;
        
        //TODO: TTL should be implements from settings!
        var ttl = new TimeSpan(0,0,3,0,0);
        await context.SetAsync($"{Prefix}{id}", conversion, ttl);
    }

    public async Task<CurrencyConversion> GetCurrencyConversionAsync(string id, CancellationToken token)
    {
        //if (token.IsCancellationRequested) return;
        
        var currency = await context.GetAsync<CurrencyConversion>($"{Prefix}{id}");
        return currency;
    }
}