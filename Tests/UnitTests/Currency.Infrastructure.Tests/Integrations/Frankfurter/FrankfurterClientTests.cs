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