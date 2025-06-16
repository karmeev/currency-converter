using System.Net.Http.Headers;

namespace Currency.IntegrationTests.Api.Controllers;

[TestFixture]
[Category("Integration")]
public class CurrencyControllerTests
{
    private HttpClient _client;

    [SetUp]
    public void Setup()
    {
        _client = ApiTestFixture.Client;
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "valid-jwt-token");
    }

    [Test]
    public async Task GetLatestExchangeRates_ShouldReturnRates()
    {
        var response = await _client.GetAsync("/api/v1/Currency/latest?currency=EUR");

        Assert.That(response.IsSuccessStatusCode, Is.True);
    }

    [Test]
    public async Task ConvertCurrency_ShouldReturnResult()
    {
        var url = "/api/v1/Currency/convert?amount=100&fromCurrency=USD&toCurrency=EUR";

        var response = await _client.PostAsync(url, null);

        Assert.That(response.IsSuccessStatusCode, Is.True);
    }
}
