using Currency.Infrastructure.Contracts.Integrations;

namespace Currency.Api.Configurations;

public static class ThirdPartyConfiguration
{
    public static void ConfigureThirdParty(this IServiceCollection services)
    {
        services.AddHttpClient(IntegrationConst.Frankfurter, client =>
        {
            client.BaseAddress = new Uri("https://api.frankfurter.app");
            client.Timeout = TimeSpan.FromSeconds(5);
        });
    }
}