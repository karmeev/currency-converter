using Currency.Facades.Contracts;
using Currency.Facades.Contracts.Requests;
using Currency.Facades.Contracts.Responses;
using Currency.Services.Contracts.Domain;

namespace Currency.Facades;

internal class CurrencyFacade(
    IConverterService converterService): ICurrencyFacade
{
    // public async Task RetrieveLatestExchangeRatesAsync()
    // {
    //     var currency = "USD";
    //
    //     //validation here, then
    //     await exchangeRatesService.GetLatestExchangeRates(currency);
    // }
    //

    public async Task<RetrieveLatestExchangeRatesResponse> RetrieveLatestExchangeRatesAsync(string currency, 
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return new RetrieveLatestExchangeRatesResponse();
    }

    public async Task<GetExchangeRatesHistoryResponse> GetExchangeRatesHistoryAsync(GetExchangeRateHistoryRequest request, 
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return new GetExchangeRatesHistoryResponse();
    }

    public async Task<ConvertToCurrencyResponse> ConvertToCurrencyAsync(ConvertCurrencyRequest request, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        //validation here
        
        var result = await converterService.ConvertToCurrency(request.Amount, request.FromCurrency, 
            request.ToCurrency, ct);
        
        return new ConvertToCurrencyResponse
        {
            Amount = result.Amount,
            Currency = result.Currency,
        };
    }
}