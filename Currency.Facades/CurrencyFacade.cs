using System.Threading.Channels;
using Currency.Common.Pagination;
using Currency.Domain.Rates;
using Currency.Facades.Contracts;
using Currency.Facades.Contracts.Dtos;
using Currency.Facades.Contracts.Exceptions;
using Currency.Facades.Contracts.Requests;
using Currency.Facades.Contracts.Responses;
using Currency.Facades.Converters;
using Currency.Facades.Validators;
using Currency.Services.Contracts.Domain;

namespace Currency.Facades;

internal class CurrencyFacade(
    IConverterService converterService,
    IExchangeRatesService exchangeRatesService,
    ChannelWriter<ExchangeRatesHistory> channel): ICurrencyFacade
{
    public async Task<RetrieveLatestExchangeRatesResponse> RetrieveLatestExchangeRatesAsync(string currency, 
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        var validationResult = ExchangeRatesValidator.ValidateRequest(currency);
        if (!validationResult.IsValid)
        {
            return ValidationException.Throw<RetrieveLatestExchangeRatesResponse>(validationResult.Message);
        }
        
        //TODO: what about cache?
        
        var rates = await exchangeRatesService.GetLatestExchangeRates(currency, ct);
        
        return new RetrieveLatestExchangeRatesResponse
        {
            CurrentCurrency = rates.CurrentCurrency,
            LastDate = rates.LastDate,
            Rates = rates.Rates
        };
    }

    public async Task<GetExchangeRatesHistoryResponse> GetExchangeRatesHistoryAsync(GetExchangeRateHistoryRequest request, 
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var validationResult = ExchangeRatesValidator.ValidateRequest(request, out var errors);
        if (!validationResult.IsValid)
        {
            return ValidationException.Throw<GetExchangeRatesHistoryResponse>(validationResult.Message, errors);
        }
        
        //TODO: fix it
        var startDate = new DateTime(request.StartDate.Year, request.StartDate.Month, request.StartDate.Day);
        var endDate = new DateTime(request.EndDate.Year, request.EndDate.Month, request.EndDate.Day);
        //
        
        var existedHistory = await exchangeRatesService.GetExistedRatesHistory(
            request.Currency, startDate, endDate, request.Page, request.PageSize, ct);
        if (existedHistory.Count > 0)
        {
            var existedParts = DtoConverter.ConvertToRatesHistoryPartDto(existedHistory);
            var existedPage = PagedList<RatesHistoryPartDto>.Create(existedParts, 
                request.Page, request.PageSize);
            
            return GetExchangeRatesHistoryResponse.BuildResponse(request.Currency, startDate, endDate, existedPage);
        }
        
        var history = await exchangeRatesService.GetExchangeRatesHistory(request.Currency, 
            startDate, endDate, ct);

        await channel.WriteAsync(history, CancellationToken.None);
        
        var parts = DtoConverter.ConvertToRatesHistoryPartDto(history);
        var page = PagedList<RatesHistoryPartDto>.CreateFromRaw(parts, request.Page, 
            request.PageSize);
        
        return GetExchangeRatesHistoryResponse.BuildResponse(request.Currency, startDate, endDate, page);
    }

    public async Task<ConvertToCurrencyResponse> ConvertToCurrencyAsync(ConvertCurrencyRequest request, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        var validationResult = ExchangeRatesValidator.ValidateRequest(request, out var errors);
        if (!validationResult.IsValid)
        {
            return ValidationException.Throw<ConvertToCurrencyResponse>(validationResult.Message, errors);
        }
        
        //TODO: what about cache?
        
        var result = await converterService.ConvertToCurrency(request.Amount, request.FromCurrency, 
            request.ToCurrency, ct);
        
        return new ConvertToCurrencyResponse
        {
            Amount = result.Amount,
            Currency = result.Currency,
        };
    }
}