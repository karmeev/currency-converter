using Currency.Domain.Operations;

namespace Currency.Services.Contracts.Domain;

public interface IConverterService
{
    Task<CurrencyConversion> ConvertToCurrency(decimal amount, string currency, string requestedCurrency,
        CancellationToken token = default);
}