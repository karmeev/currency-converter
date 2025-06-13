using Currency.Domain.Rates;
using Currency.Infrastructure.Contracts.Integrations.Providers.Base.Requests;

namespace Currency.Infrastructure.Contracts.Integrations.Providers.Base;

public interface ICurrencyProvider
{
    Task<ExchangeRates> GetLatestAsync(IGetLatestRequest request, CancellationToken token = default);

    Task<ExchangeRates> GetLatestForCurrenciesAsync(IGetLatestForCurrenciesRequest request,
        CancellationToken token = default);

    Task<ExchangeRatesHistory> GetHistoryAsync(IGetHistoryRequest request, CancellationToken token = default);
}