using Autofac;
using Currency.Infrastructure.Contracts.Databases;
using Currency.Infrastructure.Settings;
using StackExchange.Redis;

namespace Currency.Infrastructure.Redis;

internal static class Registry
{
    public static void Register(ContainerBuilder container, RedisSettings settings)
    {
        container.RegisterInstance(settings).AsSelf().SingleInstance();
        
        container.Register(_ => ConnectionMultiplexer.Connect(settings.ConnectionString))
            .As<IConnectionMultiplexer>()
            .SingleInstance();

        container.RegisterType<RedisContext>().As<IRedisContext>();
    }
}