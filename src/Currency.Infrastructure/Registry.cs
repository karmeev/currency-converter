using Autofac;
using Currency.Common.Providers;
using Currency.Infrastructure.Auth;
using Currency.Infrastructure.Contracts.Auth;
using Currency.Infrastructure.Contracts.Databases.Redis;
using Currency.Infrastructure.Contracts.JwtBearer;
using Currency.Infrastructure.Integrations.Providers;
using Currency.Infrastructure.Integrations.Providers.Frankfurter;
using Currency.Infrastructure.JwtBearer;
using Currency.Infrastructure.Redis;
using Currency.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Currency.Infrastructure;

public static class Registry
{
    public static void RegisterDependencies(ContainerBuilder container)
    {
        container.RegisterType<SecretHasher>().As<ISecretHasher>().SingleInstance();
        container.RegisterType<JwtTokenGenerator>().As<IJwtTokenGenerator>().SingleInstance();
        
        container.Register(c =>
            {
                var connectionString = c.Resolve<InfrastructureSettings>().RedisSettings.ConnectionString;
                return ConnectionMultiplexer.Connect(connectionString);
            })
            .As<IConnectionMultiplexer>()
            .SingleInstance();

        container.RegisterType<RedisContext>().As<IRedisContext>();
        
        CurrencyProvidersFactory.RegisterProviders(container);

        container.Register<IFrankfurterClient>(ctx =>
            {
                var httpClientFactory = ctx.Resolve<IHttpClientFactory>();
                var client = httpClientFactory.CreateClient(ProvidersConst.Frankfurter);
                var settings = ctx.Resolve<InfrastructureSettings>().Integrations.Frankfurter;

                client.BaseAddress = settings.BaseAddressUri;
                client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
                
                var logger = ctx.Resolve<ILogger<FrankfurterClient>>();

                return new FrankfurterClient(client, logger);
            })
            .As<IFrankfurterClient>()
            .InstancePerLifetimeScope();
    }
}