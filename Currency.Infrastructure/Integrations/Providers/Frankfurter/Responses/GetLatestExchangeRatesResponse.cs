using Newtonsoft.Json;

namespace Currency.Infrastructure.Integrations.Providers.Frankfurter.Responses;

public class GetLatestExchangeRatesResponse
{
    [JsonProperty("amount")] public decimal Amount { get; set; }

    [JsonProperty("base")] public string Base { get; set; } = default!;

    [JsonProperty("rates")] public Dictionary<string, decimal> Rates { get; set; }
}