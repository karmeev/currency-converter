using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Currency.Infrastructure.Integrations.Providers.Frankfurter.Converters;

internal class RatesDateOnlyConverter : JsonConverter<Dictionary<DateOnly, Dictionary<string, decimal>>>
{
    private const string Format = "yyyy-MM-dd";

    public override Dictionary<DateOnly, Dictionary<string, decimal>> ReadJson(JsonReader reader, Type objectType,
        Dictionary<DateOnly, Dictionary<string, decimal>>? existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);
        var result = new Dictionary<DateOnly, Dictionary<string, decimal>>();

        foreach (var property in obj.Properties())
            if (DateOnly.TryParseExact(property.Name, Format, CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out var date))
            {
                var innerRates = property.Value.ToObject<Dictionary<string, decimal>>(serializer);
                if (innerRates != null) result[date] = innerRates;
            }

        return result;
    }

    public override void WriteJson(JsonWriter writer, Dictionary<DateOnly, Dictionary<string, decimal>>? value,
        JsonSerializer serializer)
    {
        writer.WriteStartObject();
        foreach (var kvp in value!)
        {
            writer.WritePropertyName(kvp.Key.ToString(Format));
            serializer.Serialize(writer, kvp.Value);
        }

        writer.WriteEndObject();
    }
}