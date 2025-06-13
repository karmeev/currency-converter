using Autofac;
using Currency.Infrastructure.Contracts.Integrations.Providers;
using Currency.Infrastructure.Contracts.Integrations.Providers.Base;
using Currency.Infrastructure.Contracts.Integrations.Providers.Base.Exceptions;
using Currency.Infrastructure.Contracts.Integrations.Providers.Base.Requests;
using Currency.Infrastructure.Contracts.Integrations.Providers.Frankfurter;
using Currency.Infrastructure.Contracts.Integrations.Providers.Frankfurter.Requests;
using Currency.Infrastructure.Integrations.Providers.Frankfurter;

namespace Currency.Infrastructure.Integrations.Providers;

internal class CurrencyProvidersFactory(ILifetimeScope scope): ICurrencyProvidersFactory
{
    private const string FactoryKey = "currency_provider_factory";
    
    private static readonly Dictionary<Type, Type> RequestsMaps = new();

    static CurrencyProvidersFactory()
    {
        RequestsMaps.Add(typeof(FrankfurterGetHistoryRequest), typeof(IFrankfurterProvider));
        RequestsMaps.Add(typeof(FrankfurterGetLatestForCurrenciesRequest), typeof(IFrankfurterProvider));
        RequestsMaps.Add(typeof(FrankfurterGetLatestRequest), typeof(IFrankfurterProvider));
    }
    
    public ICurrencyProvider TryGetCurrencyProvider<T>(T request) where T : ICurrencyProviderRequest
    {
        try
        {
            var providerType = RequestsMaps[typeof(T)];
            var provider = scope.ResolveKeyed(FactoryKey, providerType) as ICurrencyProvider;
            if (provider is null)
            {
                //TODO: add log here
                return ProviderException.ThrowIfProviderNotFoundByRequest<ICurrencyProvider>(
                    message: "Provider not found",
                    requestType: typeof(T));
            }
            return provider;
        }
        catch (Exception ex)
        {
            //TODO: add log here
            return ProviderException.ThrowIfResolutionFailure<ICurrencyProvider>("Provider not found", ex);
        }
    }
    
    internal static void RegisterProviders(ContainerBuilder container)
    {
        container.RegisterType<FrankfurterProvider>()
            .Keyed(FactoryKey, typeof(IFrankfurterProvider))
            .InstancePerLifetimeScope();

        container.RegisterType<CurrencyProvidersFactory>()
            .As<ICurrencyProvidersFactory>()
            .SingleInstance();
    }
}