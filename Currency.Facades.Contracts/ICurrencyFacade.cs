using Currency.Facades.Contracts.Requests;
using Currency.Facades.Contracts.Responses;

namespace Currency.Facades.Contracts;

public interface ICurrencyFacade
{
    Task<RetrieveLatestExchangeRatesResponse> RetrieveLatestExchangeRatesAsync(string currency, CancellationToken ct);
    Task<GetExchangeRatesHistoryResponse> GetExchangeRatesHistoryAsync(GetExchangeRateHistoryRequest request, 
        CancellationToken ct);
    Task<ConvertToCurrencyResponse> ConvertToCurrencyAsync(ConvertCurrencyRequest request, CancellationToken ct);
}