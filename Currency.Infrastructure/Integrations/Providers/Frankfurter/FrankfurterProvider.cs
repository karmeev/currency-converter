using Currency.Domain.Rates;
using Currency.Infrastructure.Contracts.Integrations.Providers.Base.Requests;
using Currency.Infrastructure.Contracts.Integrations.Providers.Frankfurter.Base;

namespace Currency.Infrastructure.Integrations.Providers.Frankfurter;

internal class FrankfurterProvider(IFrankfurterClient client) : IDisposable, IFrankfurterProvider
{
    public void Dispose()
    {
        client.Dispose();
    }

    public async Task<ExchangeRates> GetLatestAsync(IGetLatestRequest request, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ExchangeRates> GetLatestForCurrenciesAsync(IGetLatestForCurrenciesRequest request,
        CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ExchangeRatesHistory> GetHistoryAsync(IGetHistoryRequest request,
        CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}