using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Currency.Infrastructure.Contracts.Integrations.Providers;
using Currency.Infrastructure.Contracts.Integrations.Providers.Base;
using Currency.Infrastructure.Contracts.Integrations.Providers.Base.Exceptions;
using Currency.Infrastructure.Contracts.Integrations.Providers.Base.Requests;
using Currency.Infrastructure.Contracts.Integrations.Providers.Frankfurter.Base;
using Currency.Infrastructure.Integrations.Providers.Frankfurter;
using Microsoft.Extensions.Logging;
using FrankfurterRequests = Currency.Infrastructure.Contracts.Integrations.Providers.Frankfurter;

namespace Currency.Infrastructure.Integrations.Providers;

internal class CurrencyProvidersFactory(
    ILogger<CurrencyProvidersFactory> logger,
    ILifetimeScope scope) : ICurrencyProvidersFactory
{
    private const string FactoryKey = "currency_provider_factory";

    private static readonly Dictionary<Type, Type> RequestsMaps = new();

    static CurrencyProvidersFactory()
    {
        RequestsMaps.Add(typeof(FrankfurterRequests.GetHistoryRequest), typeof(IFrankfurterProvider));
        RequestsMaps.Add(typeof(FrankfurterRequests.GetLatestForCurrenciesRequest), typeof(IFrankfurterProvider));
        RequestsMaps.Add(typeof(FrankfurterRequests.GetLatestRequest), typeof(IFrankfurterProvider));
    }
    
    public ICurrencyProvider GetCurrencyProvider<T>(T request) where T : ICurrencyProviderRequest
    {
        try
        {
            var providerType = RequestsMaps[typeof(T)];
            if (scope.ResolveKeyed(FactoryKey, providerType) is not ICurrencyProvider provider)
            {
                logger.LogError("Provider not found: {message}", typeof(T));
                return ProviderException.ThrowIfNotFound<ICurrencyProvider>("Provider not found", typeof(T));
            }
            
            return provider;
        }
        catch (Exception exception) when (exception is ComponentNotRegisteredException or DependencyResolutionException)
        {
            logger.LogError(exception, "Provider resolution failure: {message}", exception.Message);
            return ProviderException.ThrowIfResolutionFailure<ICurrencyProvider>("Provider resolution failure", 
                exception);
        }
    }

    internal static void RegisterProviders(ContainerBuilder container)
    {
        container.RegisterType<FrankfurterProvider>()
            .Keyed(FactoryKey, typeof(IFrankfurterProvider))
            .InstancePerLifetimeScope();

        container.RegisterType<CurrencyProvidersFactory>().As<ICurrencyProvidersFactory>();
    }
}