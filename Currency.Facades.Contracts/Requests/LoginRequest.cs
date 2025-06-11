using Newtonsoft.Json;

namespace Currency.Facades.Contracts.Requests;

public class LoginRequest
{
    [JsonProperty("username")] public string Username { get; set; }

    [JsonProperty("password")] public string Password { get; set; }
}