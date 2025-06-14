using Currency.Common.Pagination;
using Currency.Common.Providers;
using Currency.Data.Contracts;
using Currency.Facades.Contracts;
using Currency.Facades.Contracts.Dtos;
using Currency.Facades.Contracts.Exceptions;
using Currency.Facades.Contracts.Requests;
using Currency.Facades.Contracts.Responses;
using Currency.Facades.Converters;
using Currency.Facades.Validators;
using Currency.Services.Contracts.Application;
using Currency.Services.Contracts.Domain;
using Microsoft.Extensions.Logging;

namespace Currency.Facades;

internal class CurrencyFacade(
    ILogger<CurrencyFacade> logger,
    IConverterService converterService,
    IExchangeRatesService exchangeRatesService,
    IPublisherService publisherService,
    IExchangeRatesRepository ratesRepository): ICurrencyFacade
{
    public async Task<RetrieveLatestRatesResponse> RetrieveLatestExchangeRatesAsync(string currency, 
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        Validate(currency);
        
        var id = GetIdWithProviderPrefix(currency);
        var existedRates = await ratesRepository.GetExchangeRates(id, ct);
        if (existedRates is not null)
        {
            return new RetrieveLatestRatesResponse(existedRates.CurrentCurrency, existedRates.LastDate,
                existedRates.Rates);
        }
        
        logger.LogInformation("Cache is empty; Making a live request with currency: {currency}", currency);
        var rates = await exchangeRatesService.GetLatestExchangeRates(currency, ct);
        
        await publisherService.Publish(rates, ct);
        
        return new RetrieveLatestRatesResponse(rates.CurrentCurrency, rates.LastDate, rates.Rates);
    }

    public async Task<GetHistoryResponse> GetExchangeRatesHistoryAsync(GetHistoryRequest request, 
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        Validate(request);
        
        var existedHistory = await exchangeRatesService.GetExistedRatesHistory(
            request.Currency, request.StartDate, request.EndDate, request.Page, request.PageSize, ct);
        if (existedHistory.Count > 0)
        {
            var existedParts = DtoConverter.ConvertToRatesHistoryPartDto(existedHistory);
            var existedPage = PagedList<RatesHistoryPartDto>.Create(existedParts, 
                request.Page, request.PageSize);
            
            return new GetHistoryResponse(request.Currency, request.StartDate, request.EndDate, 
                existedPage);
        }
        
        logger.LogInformation("Cache is empty; Making a live request with currency: {currency}, Period: {start} - {end}", 
            request.Currency, request.StartDate, request.EndDate);
        
        var history = await exchangeRatesService.GetExchangeRatesHistory(request.Currency, 
            request.StartDate, request.EndDate, ct);
        
        await publisherService.Publish(history, ct);
        
        var parts = DtoConverter.ConvertToRatesHistoryPartDto(history);
        var page = PagedList<RatesHistoryPartDto>.CreateFromRaw(parts, request.Page, 
            request.PageSize);
        
        return new GetHistoryResponse(request.Currency, request.StartDate, request.EndDate, page);
    }

    public async Task<ConvertToCurrencyResponse> ConvertToCurrencyAsync(ConvertToCurrencyRequest request, 
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        Validate(request);
        
        var id = GetIdWithProviderPrefix($"{request.FromCurrency}:{request.ToCurrency}");
        var convertedCurrency = await ratesRepository.GetCurrencyConversionAsync(id, ct);
        if (convertedCurrency is not null)
        {
            return new ConvertToCurrencyResponse(convertedCurrency.Amount, convertedCurrency.ToCurrency);
        }
        
        logger.LogInformation("Cache is empty; Making a live request with currencies: {currency1} - {currency2}", 
            request.FromCurrency, request.ToCurrency);
        
        var result = await converterService.ConvertToCurrency(request.Amount, request.FromCurrency, 
            request.ToCurrency, ct);
        
        await publisherService.Publish(result, ct);
        
        return new ConvertToCurrencyResponse(result.Amount, result.ToCurrency);
    }

    private static void Validate<TRequest>(TRequest request)
    {
        var result = ExchangeRatesValidator.ValidateRequest(request, out var validationErrors);
        if (!result.IsValid)
        {
            ValidationException.Throw(result.Message, validationErrors);
        }
    }

    /// <summary>
    /// This is a helper method to get the ID with the provider prefix.
    /// After integrating the next provider, it can be abolished by the dynamic selection logic
    /// using the ICurrencyProvidersFactory.
    /// </summary>
    private static string GetIdWithProviderPrefix(string id)
    {
        return $"{ProvidersConst.Frankfurter}:{id}";
    }
}