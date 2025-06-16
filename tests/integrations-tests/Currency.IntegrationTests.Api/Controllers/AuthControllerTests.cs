using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Currency.Facades.Contracts.Requests;

namespace Currency.IntegrationTests.Api.Controllers;

[TestFixture]
//[Category("Integration")]
public class AuthControllerTests
{
    private HttpClient _client;

    [SetUp]
    public void Setup()
    {
        _client = ApiTestFixture.Client;
    }

    [Test]
    public async Task LoginAsync_ShouldReturnAccessToken_WhenValidCredentials()
    {
        var request = new LoginRequest
        {
            Username = "testuser",
            Password = "testpassword"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/Auth/login", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.That(json.GetProperty("accessToken").GetString(), Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task RefreshToken_ShouldReturnAccessToken_WhenValid()
    {
        var request = new RefreshTokenRequest
        {
            Token = "valid-refresh-token"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/Auth/refreshToken", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.That(json.GetProperty("accessToken").GetString(), Is.Not.Null.And.Not.Empty);
    }
}

