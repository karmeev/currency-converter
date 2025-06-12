using Currency.Domain.Operations;
using Currency.Infrastructure.Contracts.Integrations.Providers;
using Currency.Infrastructure.Contracts.Integrations.Providers.Frankfurter;
using Currency.Services.Contracts.Domain;

namespace Currency.Services.Domain;

internal class ConverterService(ICurrencyProvidersFactory providersFactory): IConverterService
{
    //TODO: work on naming!
    public async Task<CurrencyConversion> ConvertToCurrency(decimal amount, string currency, string requestedCurrency,
        CancellationToken token = default)
    {
        var provider = providersFactory.TryGetCurrencyProvider<IFrankfurterProvider>();
        //if not found then smth
        var rates = await provider.GetLatestForCurrenciesAsync(token);
        //if not found then smth
        var requestedAmount = Math.Round(amount * rates.Rates[requestedCurrency], 2);
        
        return new CurrencyConversion
        {
            Amount = requestedAmount,
            Currency = requestedCurrency,
        };
    }
}