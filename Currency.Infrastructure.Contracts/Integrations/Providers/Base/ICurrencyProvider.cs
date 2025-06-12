using Currency.Domain.Rates;

namespace Currency.Infrastructure.Contracts.Integrations.Providers.Base;

public interface ICurrencyProvider
{
    Task<ExchangeRates> GetLatestAsync(CancellationToken token = default);
    Task<ExchangeRates> GetLatestForCurrenciesAsync(CancellationToken token = default);
    Task<ExchangeRatesHistory> GetHistoryAsync(CancellationToken token = default);
}