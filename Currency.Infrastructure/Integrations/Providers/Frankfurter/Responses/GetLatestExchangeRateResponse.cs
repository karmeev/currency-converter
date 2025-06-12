using Newtonsoft.Json;

namespace Currency.Infrastructure.Integrations.Providers.Frankfurter.Responses;

public class GetLatestExchangeRateResponse
{
    [JsonProperty("base")] public string Base { get; set; }
    [JsonProperty("date")] public DateTime Date { get; set; }
    [JsonProperty("rates")] public Dictionary<string, decimal> Rates { get; set; }
}