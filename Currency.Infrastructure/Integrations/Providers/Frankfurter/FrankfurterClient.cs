using Currency.Infrastructure.Integrations.Providers.Frankfurter.Responses;
using Newtonsoft.Json;

namespace Currency.Infrastructure.Integrations.Providers.Frankfurter;

internal interface IFrankfurterClient: IDisposable
{
    Task<GetLatestRatesResponse> GetLatestRatesAsync(string currency, CancellationToken token = default);

    Task<GetExchangeRatesHistoryResponse> GetExchangeRatesHistoryAsync(string currency, DateOnly start, DateOnly end,
        CancellationToken token = default);

    Task<GetLatestExchangeRatesResponse> GetLatestExchangeRatesAsync(string from, string[] symbols,
        CancellationToken token = default);
} 

internal class FrankfurterClient(HttpClient client): IFrankfurterClient
{
    public async Task<GetLatestRatesResponse> GetLatestRatesAsync(string currency, CancellationToken token = default)
    {
        var builder = new UriBuilder(client.BaseAddress!)
        {
            Path = "/v1/latest",
            Query = $"base={currency}"
        };

        var response = await client.GetAsync(builder.Uri, token);
        response.EnsureSuccessStatusCode();
        
        return await ReadAndDeserializeAsync<GetLatestRatesResponse>(response.Content, ct: token);
    }

    public async Task<GetExchangeRatesHistoryResponse> GetExchangeRatesHistoryAsync(string currency, DateOnly start, DateOnly end, 
        CancellationToken token = default)
    {
        var builder = new UriBuilder(client.BaseAddress!)
        {
            Path = $"/v1/{start:yyyy-MM-dd}..{end:yyyy-MM-dd}",
            Query = $"base={currency}"
        };

        var response = await client.GetAsync(builder.Uri, token);
        response.EnsureSuccessStatusCode();
        
        return await ReadAndDeserializeAsync<GetExchangeRatesHistoryResponse>(response.Content, ct: token);
    }

    public async Task<GetLatestExchangeRatesResponse> GetLatestExchangeRatesAsync(string from, string[] symbols, 
        CancellationToken token = default)
    {
        var builder = new UriBuilder(client.BaseAddress!)
        {
            Path = "/v1/latest",
            Query = $"base={from}&symbols={SymbolsToQuery(symbols)}"
        };
        
        var response = await client.GetAsync(builder.Uri, token);
        response.EnsureSuccessStatusCode();
        
        return await ReadAndDeserializeAsync<GetLatestExchangeRatesResponse>(response.Content, ct: token);
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
            throw new OperationCanceledException("Deserialization resulted in null.", ct);
        }
        
        return result;
    }
    
    public void Dispose()
    {
        client.Dispose();
    }
}