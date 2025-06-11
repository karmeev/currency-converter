using Autofac;
using Currency.Infrastructure.Contracts.Integrations;
using Currency.Infrastructure.Integrations.Frankfurter;
using Currency.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Currency.Infrastructure.Integrations;

internal static class Registry
{
    public static void RegisterDependencies(ContainerBuilder container)
    {
        container.RegisterFrankfurter();
    }

    private static void RegisterFrankfurter(this ContainerBuilder container)
    {
        container.Register<IFrankfurterClient>(ctx =>
            {
                var httpClientFactory = ctx.Resolve<IHttpClientFactory>();
                var settingsMonitor = ctx.Resolve<IOptionsMonitor<FrankfurterSettings>>();
                
                var client = httpClientFactory.CreateClient(IntegrationConst.Frankfurter);
                var settings = settingsMonitor.CurrentValue;

                client.BaseAddress = settings.BaseAddressUri;
                client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);

                return new FrankfurterClient(client);
            })
            .As<IFrankfurterClient>()
            .InstancePerLifetimeScope(); 
    }
}