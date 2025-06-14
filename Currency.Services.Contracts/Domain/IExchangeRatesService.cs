using Currency.Data.Contracts.Entries;
using Currency.Domain.Rates;

namespace Currency.Services.Contracts.Domain;

public interface IExchangeRatesService
{
    Task<ExchangeRates> GetLatestExchangeRates(string currency, CancellationToken ct = default);

    Task<ExchangeRatesHistory> GetExchangeRatesHistory(string currency, DateTime start, DateTime end,
        CancellationToken ct = default);

    Task<List<ExchangeRateEntry>> GetExistedRatesHistory(string currency, DateTime start, DateTime end,
        int page, int size, CancellationToken ct = default);
}