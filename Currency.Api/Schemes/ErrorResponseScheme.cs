using Newtonsoft.Json;

namespace Currency.Api.Schemes;

public class ErrorResponseScheme
{
    [JsonProperty("error")] public string Error { get; set; }
    [JsonProperty("message")] public string Message { get; set; }
}