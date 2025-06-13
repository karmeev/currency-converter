using Currency.Domain.Operations;
using Currency.Infrastructure.Contracts.Integrations.Providers;
using Currency.Services.Contracts.Domain;
using Frankfurter = Currency.Infrastructure.Contracts.Integrations.Providers.Frankfurter;

namespace Currency.Services.Domain;

internal class ConverterService(ICurrencyProvidersFactory factory) : IConverterService
{
    public async Task<CurrencyConversion> ConvertToCurrency(decimal amount, string currency, string requestedCurrency,
        CancellationToken token = default)
    {
        var request = new Frankfurter.GetLatestForCurrenciesRequest(currency, [requestedCurrency]);
        var provider = factory.GetCurrencyProvider(request);
        var rates = await provider.GetLatestForCurrenciesAsync(request, token);
        //if not found then smth
        var requestedAmount = Math.Round(amount * rates.Rates[requestedCurrency], 2);

        return new CurrencyConversion
        {
            Amount = requestedAmount,
            Currency = requestedCurrency
        };
    }
}