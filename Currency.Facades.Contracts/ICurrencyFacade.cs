using Currency.Facades.Contracts.Requests;
using Currency.Facades.Contracts.Responses;

namespace Currency.Facades.Contracts;

public interface ICurrencyFacade
{
    Task<RetrieveLatestRatesResponse> RetrieveLatestExchangeRatesAsync(string currency, CancellationToken ct);
    Task<GetHistoryResponse> GetExchangeRatesHistoryAsync(GetHistoryRequest request, 
        CancellationToken ct);
    Task<ConvertToCurrencyResponse> ConvertToCurrencyAsync(ConvertToCurrencyRequest request, CancellationToken ct);
}