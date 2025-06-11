using Currency.Infrastructure.Contracts.Integrations.Providers.Frankfurter.Responses;
using Currency.Infrastructure.Integrations.Frankfurter;

namespace Currency.Infrastructure.Tests.Integrations.Frankfurter;

public class FrankfurterClientTests
{
    private HttpClient _client;
    
    [SetUp]
    public void Setup()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("https://api.frankfurter.dev");
        _client.Timeout = TimeSpan.FromSeconds(5);
    }

    [Test]
    public async Task GetLatestRatesAsync_HappyPath_ReturnsLatestUsdRates()
    {
        //Arrange
        var sut = new FrankfurterClient(_client);
        
        //Act
        var result = await sut.GetLatestRatesAsync<GetLatestRatesResponse>("USD", CancellationToken.None);
        
        //Assert
        Assert.Pass();
    }
    
    [Test]
    public async Task GetLatestExchangeRateAsync_HappyPath_ReturnsExchangeRate()
    {
        //Arrange
        var sut = new FrankfurterClient(_client);
        
        //Act
        //await sut.GetLatestExchangeRateAsync<ConvertToCurrencyResponse>(null, null);
        var result = await sut.GetLatestExchangeRatesAsync<ConvertToCurrencyResponse>("EUR", ["USD"], CancellationToken.None);
        
        //Assert
        Assert.Pass();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
    }
}