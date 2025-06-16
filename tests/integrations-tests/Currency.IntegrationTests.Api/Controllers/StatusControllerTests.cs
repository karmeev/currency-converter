using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Currency.IntegrationTests.Api.Controllers;

[TestFixture]
//[Category("Integration")]
public class StatusControllerTests
{
    private HttpClient _client;

    [SetUp]
    public void Setup()
    {
        _client = ApiTestFixture.Client;
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "admin-jwt-token");
    }

    [Test]
    public async Task GetStatus_ShouldReturnHealthy()
    {
        var response = await _client.GetAsync("/api/v2/Status/health");

        Assert.That(response.IsSuccessStatusCode, Is.True);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.That(json.GetProperty("Status").GetString(), Is.EqualTo("Healthy"));
    }
}
