using Currency.Infrastructure.Integrations.Providers.Frankfurter.Converters;
using Newtonsoft.Json;

namespace Currency.Infrastructure.Integrations.Providers.Frankfurter.Responses;

public class GetLatestExchangeRatesResponse
{
    [JsonProperty("amount")] public decimal Amount { get; set; }

    [JsonProperty("base")] public string Base { get; set; } = default!;
    
    [JsonProperty("date")]
    [JsonConverter(typeof(DateOnlyConverter))]
    public DateOnly Date { get; set; }

    [JsonProperty("rates")] public Dictionary<string, decimal> Rates { get; set; }
}