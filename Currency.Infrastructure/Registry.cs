using Autofac;
using Currency.Infrastructure.Auth;
using Currency.Infrastructure.Contracts.Auth;
using Currency.Infrastructure.Contracts.JwtBearer;
using Currency.Infrastructure.JwtBearer;
using Currency.Infrastructure.Settings;

namespace Currency.Infrastructure;

public static class Registry
{
    public static void RegisterDependencies(ContainerBuilder container)
    {
        container.RegisterType<SecretHasher>().As<ISecretHasher>().SingleInstance();
        container.RegisterType<JwtTokenGenerator>().As<IJwtTokenGenerator>().SingleInstance();
        Redis.Registry.RegisterDependencies(container);
        Integrations.Registry.RegisterDependencies(container);
    }
}