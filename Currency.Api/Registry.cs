using Autofac;
using Currency.Infrastructure.Settings;
using Currency.Services.Application.Settings;
using Microsoft.Extensions.Options;

namespace Currency.Api;

public static class Registry
{
    public static void RegisterDependencies(ContainerBuilder container)
    {
        PopulateSettings(container);
        Facades.Registry.RegisterDependencies(container);
        Services.Registry.RegisterDependencies(container);
        Data.Registry.RegisterDependencies(container);
        Infrastructure.Registry.RegisterDependencies(container);
    }

    private static void PopulateSettings(ContainerBuilder container)
    {
        container.Register(c => new ServicesSettings
        {
            RefreshTokenTtlInDays = c.Resolve<IOptions<JwtSettings>>().Value.RefreshTokenTtlInDays,
        }).As<ServicesSettings>().SingleInstance();
        
        container.Register(c => new InfrastructureSettings
        {
            JwtSettings = c.Resolve<IOptions<JwtSettings>>().Value,
            RedisSettings = c.Resolve<IOptions<RedisSettings>>().Value,
            Integrations = new IntegrationsSettings
            {
                Frankfurter = c.Resolve<IOptionsMonitor<FrankfurterSettings>>().CurrentValue
            }
        }).As<InfrastructureSettings>().SingleInstance();
    }
}