using System.Diagnostics;
using Currency.Infrastructure.Contracts.Integrations.Providers.Base.Exceptions;
using Currency.Infrastructure.Integrations.Providers.Frankfurter.Responses;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly.CircuitBreaker;

namespace Currency.Infrastructure.Integrations.Providers.Frankfurter;

internal interface IFrankfurterClient : IDisposable
{
    Task<GetLatestExchangeRateResponse> GetLatestExchangeRateAsync(string currency, CancellationToken token = default);

    Task<GetLatestExchangeRatesResponse> GetLatestExchangeRatesAsync(string from, string[] symbols,
        CancellationToken token = default);

    Task<GetExchangeRatesHistoryResponse> GetExchangeRatesHistoryAsync(string currency, DateOnly start, DateOnly end,
        CancellationToken token = default);
}

internal class FrankfurterClient(
    HttpClient client, 
    ILogger<FrankfurterClient> logger) : IFrankfurterClient
{
    public async Task<GetLatestExchangeRateResponse> GetLatestExchangeRateAsync(string currency,
        CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        var uri = new UriBuilder(client.BaseAddress!)
        {
            Path = "/v1/latest",
            Query = $"base={currency}"
        }.Uri;

        WithTelemetry(uri);
        logger.LogInformation("Requesting latest exchange rate from Frankfurter. Base: {Currency}, URI: {Uri}", 
            currency, uri);

        var response = await HandleAsync(async () => await client.GetAsync(uri, token));
        return await ReadAndDeserializeAsync<GetLatestExchangeRateResponse>(response.Content, token);
    }

    public async Task<GetLatestExchangeRatesResponse> GetLatestExchangeRatesAsync(string from, string[] symbols,
        CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        var uri = new UriBuilder(client.BaseAddress!)
        {
            Path = "/v1/latest",
            Query = $"base={from}&symbols={SymbolsToQuery(symbols)}"
        }.Uri;
        
        WithTelemetry(uri);
        logger.LogInformation("Requesting latest exchange rates from Frankfurter. URI: {Uri}, " +
                              "from: {from}, symbols: {Symbols}", uri.ToString(), from, symbols);
        
        var response = await HandleAsync(async () => await client.GetAsync(uri, token));
        return await ReadAndDeserializeAsync<GetLatestExchangeRatesResponse>(response.Content, token);
    }

    public async Task<GetExchangeRatesHistoryResponse> GetExchangeRatesHistoryAsync(string currency, DateOnly start,
        DateOnly end, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        var uri = new UriBuilder(client.BaseAddress!)
        {
            Path = $"/v1/{start:yyyy-MM-dd}..{end:yyyy-MM-dd}",
            Query = $"base={currency}"
        }.Uri;
        
        WithTelemetry(uri);
        logger.LogInformation("Requesting exchange rates history from Frankfurter. Base: {Currency}, URI: {Uri}, " +
                              "start: {start:yyyy-MM-dd}, end: {end:yyyy-MM-dd}", currency, start, end, uri);
        
        var response = await HandleAsync(async () => await client.GetAsync(uri, token));
        return await ReadAndDeserializeAsync<GetExchangeRatesHistoryResponse>(response.Content, token);
    }
    
    private static string SymbolsToQuery(ReadOnlySpan<string> currencies)
    {
        if (currencies.Length == 1) return currencies[0];
        return string.Join(",", currencies);
    }

    private async Task<T> ReadAndDeserializeAsync<T>(HttpContent response, CancellationToken ct = default)
    {
        var content = await response.ReadAsStringAsync(ct);
        var result = JsonConvert.DeserializeObject<T>(content);

        if (result is null)
        {
            var ex = new InvalidOperationException("Deserialization resulted in null.");
            logger.LogError(ex, "Frankfurter API: Deserialization resulted in null.");
            throw new HttpProviderException("Frankfurter API: Unexpected response", ex);
        }

        return result;
    }

    private static void WithTelemetry(Uri uri)
    {
        Activity.Current?.SetTag("frankfurter.uri", uri.ToString());
    }
    
    private async Task<HttpResponseMessage> HandleAsync(Func<Task<HttpResponseMessage>> action)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await action();
            stopwatch.Stop();

            var traceId = Activity.Current?.TraceId.ToString() ?? "N/A";
            logger.LogInformation("TraceId: {TraceId}; Frankfurter responded with status {StatusCode} in {ElapsedMilliseconds}ms",
                traceId, response.StatusCode, stopwatch.ElapsedMilliseconds);
            
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Frankfurter request failed with status code {StatusCode}", response.StatusCode);
            }
            
            response.EnsureSuccessStatusCode();

            return response;
        }
        catch (HttpRequestException ex)
        {
            if (ex.InnerException is BrokenCircuitException bce)
            {
                logger.LogWarning(bce, "Frankfurter API circuit breaker is open. Requests are temporarily blocked.");
                throw new HttpProviderException(
                    "Frankfurter API is currently unavailable due to repeated failures. Please try again later.", bce);
            }

            logger.LogWarning(ex, "Frankfurter request failed due to network or non-success HTTP code.");
            throw;
        }
        catch (Polly.Timeout.TimeoutRejectedException ex)
        {
            logger.LogError(ex, "Frankfurter API timeout: request did not complete within the allotted time. " +
                                "Exception: {Message}", ex.Message);
            throw new HttpProviderException("Frankfurter API don't respond", ex);
        }
        catch (BrokenCircuitException ex)
        {
            logger.LogWarning(ex, "Frankfurter API circuit breaker is open. Requests are temporarily blocked.");
            throw new HttpProviderException(
                "Frankfurter API is currently unavailable due to repeated failures. Please try again later.", ex);
        }
    }
    
    public void Dispose()
    {
        client.Dispose();
    }
}