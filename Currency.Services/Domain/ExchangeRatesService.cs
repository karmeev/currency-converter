using Currency.Data.Contracts;
using Currency.Data.Contracts.Entries;
using Currency.Domain.Rates;
using Currency.Infrastructure.Contracts.Integrations.Providers;
using Currency.Services.Contracts.Domain;
using Frankfurter = Currency.Infrastructure.Contracts.Integrations.Providers.Frankfurter;

namespace Currency.Services.Domain;

public class ExchangeRatesService(
    ICurrencyProvidersFactory factory,
    IExchangeRatesHistoryRepository exchangeRatesHistoryRepository): IExchangeRatesService
{
    public async Task<ExchangeRates> GetLatestExchangeRates(string currency, CancellationToken ct = default)
    {
        var request = new Frankfurter.GetLatestRequest(currency);
        var provider = factory.GetCurrencyProvider(request);
        var rates = await provider.GetLatestAsync(request, ct);
        return rates;
    }

    public async Task<ExchangeRatesHistory> GetExchangeRatesHistory(string currency, DateTime start, DateTime end,
        CancellationToken ct = default)
    {
        var request = new Frankfurter.GetHistoryRequest(currency, DateOnly.FromDateTime(start), 
            DateOnly.FromDateTime(end));
        var provider = factory.GetCurrencyProvider(request);
        var history = await provider.GetHistoryAsync(request, ct);
        return history;
    }

    public async Task<List<ExchangeRateEntry>> GetExistedRatesHistory(string currency, DateTime start, DateTime end,
        int page, int size, CancellationToken ct = default)
    {
        var key = $"Frankfurter:{currency}:{start:yyyyMMddHHmmss}:{end:yyyyMMddHHmmss}";

        var entries = await exchangeRatesHistoryRepository.GetRateHistoryPagedAsync(key, page, size, ct);
        return entries
            .Where(e => e.Date >= start && e.Date <= end)
            .ToList();
    } 
}