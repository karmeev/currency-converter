using Currency.Domain.Operations;
using Currency.Domain.Rates;

namespace Currency.Data.Contracts;

#nullable enable
public interface IExchangeRatesRepository
{
    Task AddExchangeRates(string id, ExchangeRates exchangeRates, CancellationToken token);
    Task<ExchangeRates?> GetExchangeRates(string id, CancellationToken token);
    Task AddConversionResultAsync(string id, CurrencyConversion conversion, CancellationToken token);
    Task<CurrencyConversion?> GetCurrencyConversionAsync(string id, CancellationToken token);
}