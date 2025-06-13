using System.Globalization;
using Newtonsoft.Json;

namespace Currency.Infrastructure.Integrations.Providers.Frankfurter.Converters;

internal class DateOnlyConverter : JsonConverter<DateOnly>
{
    private const string Format = "yyyy-MM-dd";

    public override DateOnly ReadJson(JsonReader reader, Type objectType, DateOnly existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var dateStr = reader.Value?.ToString();
        if (dateStr is not null && DateOnly.TryParseExact(dateStr, Format, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var date)) return date;

        throw new JsonSerializationException($"Invalid date format: {dateStr}");
    }

    public override void WriteJson(JsonWriter writer, DateOnly value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString(Format));
    }
}