using Currency.Domain.Operations;
using Currency.Infrastructure.Contracts.Integrations.Providers;
using Currency.Services.Contracts.Application.Exceptions;
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
        if (!rates.Rates.TryGetValue(requestedCurrency, out var rate))
        {
            return CurrencyNotFoundException.Throw<CurrencyConversion>($"{currency} - Currency not found");
        }
        
        var requestedAmount = Math.Round(amount * rate, 2);
        return new CurrencyConversion
        {
            Provider = rates.Provider,
            Amount = requestedAmount,
            FromCurrency = currency,
            ToCurrency = requestedCurrency
        };
    }
}