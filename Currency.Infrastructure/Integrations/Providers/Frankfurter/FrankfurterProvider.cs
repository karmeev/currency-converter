using Currency.Domain.Rates;
using Currency.Infrastructure.Contracts.Integrations;
using Currency.Infrastructure.Contracts.Integrations.Providers.Base.Requests;
using Currency.Infrastructure.Contracts.Integrations.Providers.Frankfurter;
using Currency.Infrastructure.Contracts.Integrations.Providers.Frankfurter.Base;

namespace Currency.Infrastructure.Integrations.Providers.Frankfurter;

internal class FrankfurterProvider(IFrankfurterClient client) : IDisposable, IFrankfurterProvider
{
    public async Task<ExchangeRates> GetLatestAsync(IGetLatestRequest request, CancellationToken token = default)
    {
        var frankfurterRequest = (GetLatestRequest)request;
        var response = await client.GetLatestExchangeRateAsync(frankfurterRequest.Currency, token);
        
        return new ExchangeRates
        {
            CurrentCurrency = response.Base,
            LastDate = response.Date.ToDateTime(TimeOnly.MinValue),
            Rates = response.Rates
        };
    }

    public async Task<ExchangeRates> GetLatestForCurrenciesAsync(IGetLatestForCurrenciesRequest request,
        CancellationToken token = default)
    {
        var frankfurterRequest = (GetLatestForCurrenciesRequest)request;
        var response = await client.GetLatestExchangeRatesAsync(frankfurterRequest.Currency, frankfurterRequest.Symbols, 
            token);
        
        return new ExchangeRates
        {
            CurrentCurrency = response.Base,
            LastDate = response.Date.ToDateTime(TimeOnly.MinValue),
            Rates = response.Rates
        };
    }

    public async Task<ExchangeRatesHistory> GetHistoryAsync(IGetHistoryRequest request,
        CancellationToken token = default)
    {
        var frankfurterRequest = (GetHistoryRequest)request;
        var response = await client.GetExchangeRatesHistoryAsync(frankfurterRequest.Currency, frankfurterRequest.Start,
            frankfurterRequest.End, token);

        var rates = response.Rates.ToDictionary(rate => 
            rate.Key.ToDateTime(TimeOnly.MinValue), rate => rate.Value);

        return new ExchangeRatesHistory
        {
            Provider = IntegrationConst.Frankfurter,
            CurrentCurrency = response.Base,
            StartDate = response.StartDate.ToDateTime(TimeOnly.MinValue),
            EndDate = response.EndDate.ToDateTime(TimeOnly.MinValue),
            Rates = rates
        };
    }
    
    public void Dispose()
    {
        client.Dispose();
    }
}