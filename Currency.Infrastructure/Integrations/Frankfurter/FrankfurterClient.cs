using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Currency.Infrastructure.Integrations.Frankfurter;

internal interface IFrankfurterClient
{
    
} 

internal class FrankfurterClient(HttpClient client): IDisposable, IFrankfurterClient
{
    public async Task<T> GetLatestRatesAsync<T>(string @base, CancellationToken token = default)
    {
        var builder = new UriBuilder(client.BaseAddress!)
        {
            Path = "/v1/latest",
            Query = $"base={@base}"
        };

        var response = await client.GetAsync(builder.Uri, token);
        response.EnsureSuccessStatusCode();
        
        return await ReadAndDeserializeAsync<T>(response.Content, token);
    }

    public async Task<T> GetLatestExchangeRatesAsync<T>(string from, string[] symbols, 
        CancellationToken token = default)
    {
        var builder = new UriBuilder(client.BaseAddress!)
        {
            Path = "/v1/latest",
            Query = $"base={from}&symbols={SymbolsToQuery(ref symbols)}"
        };
        
        var response = await client.GetAsync(builder.Uri, token);
        response.EnsureSuccessStatusCode();
        
        return await ReadAndDeserializeAsync<T>(response.Content, token);
    }
    
    private static string SymbolsToQuery(ref string[] currencies)
    {
        if (currencies.Length == 1) return currencies[0];
        return string.Join(",", currencies);
    }

    private async Task<T> ReadAndDeserializeAsync<T>(HttpContent response, CancellationToken token)
    {
        var content = await response.ReadAsStringAsync(token);
        var result = JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (result is null)
        {
            throw new OperationCanceledException("Deserialization resulted in null.", token);
        }
        
        return result;
    }
    
    public void Dispose()
    {
        client.Dispose();
    }
}