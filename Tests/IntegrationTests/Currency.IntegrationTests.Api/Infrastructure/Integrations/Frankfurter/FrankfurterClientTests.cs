using Currency.Api.Configurations;
using Currency.Infrastructure.Contracts.Integrations;
using Currency.Infrastructure.Integrations.Frankfurter;
using Microsoft.Extensions.DependencyInjection;
using Polly.CircuitBreaker;

namespace Currency.IntegrationTests.Api.Infrastructure.Integrations.Frankfurter;

[TestFixture]
[Category("Integration tests")]
public class FrankfurterClientTests
{
    private HttpClient _client;
    private const string WireMockAddress = "http://localhost:8080";
    
    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.ConfigureThirdParty();
        services.AddOptions();
        services.AddHttpClient();
        var provider = services.BuildServiceProvider();
        
        var factory = provider.GetRequiredService<IHttpClientFactory>();
        var client = factory.CreateClient(IntegrationConst.Frankfurter);
        
        _client = client;
    }

    [Test]
    public async Task GetLatestRatesAsync_HappyPath_ReturnsLatestUsdRates()
    {
        //Arrange
        var sut = new FrankfurterClient(_client);
        
        //Act
        var result = await sut.GetLatestRatesAsync("USD", CancellationToken.None);
        
        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Base, Is.EqualTo("USD"));
            Assert.That(result.Date, Is.LessThanOrEqualTo(DateTime.UtcNow.Date));
            Assert.That(result.Rates, Is.Not.Null.And.Not.Empty);
        });
    }
    
    [Test]
    public async Task GetLatestRatesAsync_ReturnsInternalServerErrorOneTime_ShouldRetry()
    {
        //Arrange
        _client.BaseAddress = new Uri(WireMockAddress);
        var sut = new FrankfurterClient(_client);
        
        //Act
        var result = await sut.GetLatestRatesAsync("USD", CancellationToken.None);
        
        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Base, Is.EqualTo("USD"));
            Assert.That(result.Date, Is.LessThanOrEqualTo(DateTime.UtcNow.Date));
            Assert.That(result.Rates, Is.Not.Null.And.Not.Empty);
        });
    }
    
    [Test]
    public async Task GetLatestRatesAsync_ReturnsInternalServerErrorManyTime_ShouldThrowBrokenCircuitException()
    {
        //Arrange
        _client.BaseAddress = new Uri(WireMockAddress);
        var sut = new FrankfurterClient(_client);
        
        // Act
        for (int i = 0; i < 10; i++)
        {
            try { await sut.GetLatestRatesAsync("EUR"); } catch { }
        }

        // Assert
        Assert.ThrowsAsync<BrokenCircuitException<HttpResponseMessage>>(async () => 
        {
            await sut.GetLatestRatesAsync("EUR");
        });
    }
    
    [Test]
    public async Task GetExchangeRatesHistoryAsync_HappyPath_ReturnsHistory()
    {
        //Arrange
        var sut = new FrankfurterClient(_client);
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-1));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        
        //Act
        var result = await sut.GetExchangeRatesHistoryAsync("USD", startDate,
            endDate, CancellationToken.None);
        
        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Amount, Is.EqualTo(1));
            Assert.That(result.Base, Is.EqualTo("USD"));
            Assert.That(result.StartDate, Is.EqualTo(startDate));
            Assert.That(result.EndDate, Is.LessThanOrEqualTo(endDate));
            Assert.That(result.Rates, Is.Not.Null.And.Not.Empty);
            Assert.That(result.Rates, Is.Not.Empty, "Expected at least one entry with USD rate.");
        });
    }
    
    [Test]
    public async Task GetLatestExchangeRatesAsync_HappyPath_ReturnsRates()
    {
        //Arrange
        var sut = new FrankfurterClient(_client);
        
        //Act
        var result = await sut.GetLatestExchangeRatesAsync("EUR", ["USD"], CancellationToken.None);
        
        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Amount, Is.EqualTo(1));
            Assert.That(result.Base, Is.EqualTo("EUR"));
            Assert.That(result.Rates, Is.Not.Null.And.Not.Empty);
            Assert.That(result.Rates.Keys.Any(r => r == "USD"), Is.True, 
                "Expected at least one entry with USD rate.");
        });
    }
    
    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
    }
}