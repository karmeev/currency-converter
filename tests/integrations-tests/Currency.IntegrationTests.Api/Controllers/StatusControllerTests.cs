using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Currency.Facades.Contracts.Requests;

namespace Currency.IntegrationTests.Api.Controllers;

[TestFixture]
[Category("Integration")]
public class StatusControllerTests
{
    private HttpClient _client;

    [SetUp]
    public async Task Setup()
    {
        _client = ApiTestFixture.Client;
        await Task.Delay(2000);
        
        var request = new LoginRequest
        {
            Username = "test-user",
            Password = "my_test_password"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/Auth/login", request);
        var token = (await response.Content.ReadFromJsonAsync<JsonElement>())
            .GetProperty("accessToken")
            .GetString();
        
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    [Test]
    public async Task GetStatus_ShouldReturnHealthy()
    {
        var response = await _client.GetAsync("/api/v2/status/health");

        Assert.That(response.IsSuccessStatusCode, Is.True);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.That(json.GetProperty("status").GetString(), Is.EqualTo("Healthy"));
    }
}
