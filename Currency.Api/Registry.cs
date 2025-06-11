using Autofac;
using Currency.Api.Models;
using Currency.Api.Settings;
using Currency.Services.Application.Settings;

namespace Currency.Api;

public static class Registry
{
    public static void RegisterDependencies(ContainerBuilder container, ApiSettings settings,
        ConfigurationManager configurationManager)
    {
        Facades.Registry.RegisterDependencies(container);
        Services.Registry.RegisterDependencies(container, new ServicesSettings
        {
            RefreshTokenTtlInDays = settings.InfrastructureSettings.JwtSettings.RefreshTokenTtlInDays
        });
        Data.Registry.RegisterDependencies(container, configurationManager.Get<UsersModel>().Users);
        Infrastructure.Registry.RegisterDependencies(container, settings.InfrastructureSettings);
    }
}