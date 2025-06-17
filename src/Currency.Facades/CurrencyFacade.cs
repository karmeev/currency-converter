using Currency.Common.Pagination;
using Currency.Common.Providers;
using Currency.Data.Contracts;
using Currency.Domain.Extensions;
using Currency.Domain.Rates;
using Currency.Facades.Contracts;
using Currency.Facades.Contracts.Exceptions;
using Currency.Facades.Contracts.Requests;
using Currency.Facades.Contracts.Responses;
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
        
        logger.LogInformation("started; retrieve latests exchange rates for currency: {currency}", currency);

        Validate(currency);
        
        var id = GetIdWithProviderPrefix(currency);
        var existedRates = await ratesRepository.GetExchangeRates(id, ct);
        if (existedRates is not null)
        {
            return new RetrieveLatestRatesResponse(existedRates.CurrentCurrency, existedRates.LastDate,
                existedRates.Rates);
        }
        
        logger.LogInformation("processing; Cache is empty; Making a live request with currency: {currency}", currency);
        var rates = await exchangeRatesService.GetLatestExchangeRates(currency, ct);
        
        await publisherService.Publish(rates, ct);
        
        logger.LogInformation("completed; retrieve latests exchange rates for currency: {currency}", currency);
        return new RetrieveLatestRatesResponse(rates.CurrentCurrency, rates.LastDate, rates.Rates);
    }

    public async Task<GetHistoryResponse> GetExchangeRatesHistoryAsync(GetHistoryRequest request, 
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        logger.LogInformation("started; get exchange rates history for: {currency}", request.Currency);
        
        Validate(request);
        
        var existedHistory = await exchangeRatesService.GetExistedRatesHistory(
            request.Currency, request.StartDate, request.EndDate, request.Page, request.PageSize, ct);
        if (existedHistory.Any())
        {
            var existedPage = PagedList<ExchangeRatesHistoryPart>.Create(existedHistory, 
                request.Page, request.PageSize);
            
            return new GetHistoryResponse(request.Currency, request.StartDate, request.EndDate, 
                existedPage);
        }
        
        logger.LogInformation("processing; Cache is empty; Making a live request with currency: {currency}, " +
                              "Period: {start} - {end}", 
            request.Currency, request.StartDate, request.EndDate);
        
        var history = await exchangeRatesService.GetExchangeRatesHistory(request.Currency, 
            request.StartDate, request.EndDate, ct);
        
        await publisherService.Publish(history, ct);
        
        var parts = history.ToPartOfHistory();
        var page = PagedList<ExchangeRatesHistoryPart>.CreateFromRaw(parts, request.Page, 
            request.PageSize);
        
        logger.LogInformation("completed; get exchange rates history for: {currency}", request.Currency);
        
        return new GetHistoryResponse(request.Currency, request.StartDate, request.EndDate, page);
    }

    public async Task<ConvertToCurrencyResponse> ConvertToCurrencyAsync(ConvertToCurrencyRequest request, 
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        logger.LogInformation("started; convert from: {currency} to: {currency2}", request.FromCurrency,
            request.ToCurrency);
        
        Validate(request);
        
        var id = GetIdWithProviderPrefix($"{request.FromCurrency}:{request.ToCurrency}");
        var convertedCurrency = await ratesRepository.GetCurrencyConversionAsync(id, ct);
        if (convertedCurrency is not null)
        {
            return new ConvertToCurrencyResponse(convertedCurrency.Amount, convertedCurrency.ToCurrency);
        }
        
        logger.LogInformation("processing; Cache is empty; Making a live request with currencies: {currency1} - {currency2}", 
            request.FromCurrency, request.ToCurrency);
        
        var result = await converterService.ConvertToCurrency(request.Amount, request.FromCurrency, 
            request.ToCurrency, ct);
        
        await publisherService.Publish(result, ct);
        
        logger.LogInformation("completed; convert from: {currency} to: {currency2}", request.FromCurrency,
            request.ToCurrency);
        
        return new ConvertToCurrencyResponse(result.Amount, result.ToCurrency);
    }

    private void Validate<TRequest>(TRequest request)
    {
        var result = ExchangeRatesValidator.ValidateRequest(request, out var validationErrors);
        if (!result.IsValid)
        {
            logger.LogInformation("failed; validation failed for request: {req}", request.GetType());
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