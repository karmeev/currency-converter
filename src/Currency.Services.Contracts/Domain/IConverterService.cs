using Currency.Domain.Operations;
using Currency.Services.Contracts.Application.Exceptions;

namespace Currency.Services.Contracts.Domain;

public interface IConverterService
{
    /// <summary>
    /// Converts a specified monetary amount from one currency to another.
    /// </summary>
    /// <param name="amount">The amount of money to convert.</param>
    /// <param name="currency">The ISO code of the source currency (e.g., "USD").</param>
    /// <param name="requestedCurrency">The ISO code of the target currency (e.g., "EUR").</param>
    /// <param name="token">A cancellation token to cancel the operation (optional).</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="CurrencyConversion"/> 
    /// object representing the converted amount and related metadata.
    /// </returns>
    /// <exception cref="CurrencyNotFoundException">
    /// Thrown when either the source or the target currency code is not recognized.
    /// </exception>
    Task<CurrencyConversion> ConvertToCurrency(decimal amount, string currency, string requestedCurrency,
        CancellationToken token = default);
}