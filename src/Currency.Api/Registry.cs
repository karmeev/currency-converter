using Autofac;
using Currency.Data.Settings;
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
        //static: Refresh Token is long-lived often
        container.Register(c => new ServicesSettings
        {
            RefreshTokenTtlInDays = c.Resolve<IOptions<JwtSettings>>().Value.RefreshTokenTtlInDays,
        }).As<ServicesSettings>().SingleInstance();

        container.RegisterType<InfrastructureSettings>()
            .AsSelf()
            .InstancePerLifetimeScope();
        
        container.RegisterType<DataSettings>()
            .AsSelf()
            .InstancePerLifetimeScope();
    }
}