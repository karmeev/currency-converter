using Autofac;
using Currency.Infrastructure.Contracts.Databases;
using Currency.Infrastructure.Contracts.Databases.Redis;
using Currency.Infrastructure.Settings;
using StackExchange.Redis;

namespace Currency.Infrastructure.Redis;

internal static class Registry
{
    public static void RegisterDependencies(ContainerBuilder container)
    {
        container.Register(c =>
            {
                var connectionString = c.Resolve<InfrastructureSettings>().RedisSettings.ConnectionString;
                return ConnectionMultiplexer.Connect(connectionString);
            })
            .As<IConnectionMultiplexer>()
            .SingleInstance();

        container.RegisterType<RedisContext>().As<IRedisContext>();
    }
}