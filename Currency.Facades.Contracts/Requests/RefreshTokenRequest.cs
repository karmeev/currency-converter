using Newtonsoft.Json;

namespace Currency.Facades.Contracts.Requests;

public class RefreshTokenRequest
{
    [JsonProperty("token")]
    public string Token { get; set; }
}