using Autofac;
using Currency.Api.Settings;

namespace Currency.Api;

public static class Registry
{
    public static void RegisterDependencies(ContainerBuilder container, ApiSettings settings)
    {
        Facades.Registry.RegisterDependencies(container);
        Services.Registry.RegisterDependencies(container);
    }
}