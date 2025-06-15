using Currency.Infrastructure.Integrations.Providers.Frankfurter.Converters;
using Newtonsoft.Json;

namespace Currency.Infrastructure.Integrations.Providers.Frankfurter.Responses;

public class GetExchangeRatesHistoryResponse
{
    [JsonProperty("amount")] public decimal Amount { get; set; }

    [JsonProperty("base")] public string Base { get; set; }

    [JsonProperty("start_date")]
    [JsonConverter(typeof(DateOnlyConverter))]
    public DateOnly StartDate { get; set; }

    [JsonProperty("end_date")]
    [JsonConverter(typeof(DateOnlyConverter))]
    public DateOnly EndDate { get; set; }

    [JsonProperty("rates")]
    [JsonConverter(typeof(RatesDateOnlyConverter))]
    public Dictionary<DateOnly, Dictionary<string, decimal>> Rates { get; set; } = new();
}