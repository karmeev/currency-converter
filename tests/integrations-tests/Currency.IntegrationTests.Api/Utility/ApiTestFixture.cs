using Microsoft.Extensions.Configuration;

namespace Currency.IntegrationTests.Api;

[SetUpFixture]
public class ApiTestFixture
{
    public static HttpClient Client { get; private set; }

    [OneTimeSetUp]
    public void GlobalSetup()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        
        Client = new HttpClient
        {
            BaseAddress = new Uri(config.GetSection("ApiUrl").Value)
        };
    }

    [OneTimeTearDown]
    public void GlobalTeardown()
    {
        Client.Dispose();
    }
}

