using System.Text.Json.Serialization;

namespace Currency.Facades.Contracts.Requests;

public class RefreshTokenRequest
{
    [JsonPropertyName("token")] public string Token { get; set; }
}