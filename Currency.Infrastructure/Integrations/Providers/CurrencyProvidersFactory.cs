using Autofac;
using Currency.Infrastructure.Contracts.Integrations.Providers;
using Currency.Infrastructure.Contracts.Integrations.Providers.Base;
using Currency.Infrastructure.Contracts.Integrations.Providers.Base.Exceptions;
using Currency.Infrastructure.Contracts.Integrations.Providers.Base.Requests;
using Currency.Infrastructure.Contracts.Integrations.Providers.Frankfurter.Base;
using Currency.Infrastructure.Integrations.Providers.Frankfurter;
using FrankfurterRequests = Currency.Infrastructure.Contracts.Integrations.Providers.Frankfurter;

namespace Currency.Infrastructure.Integrations.Providers;

internal class CurrencyProvidersFactory(ILifetimeScope scope) : ICurrencyProvidersFactory
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
            var provider = scope.ResolveKeyed(FactoryKey, providerType) as ICurrencyProvider;
            if (provider is null)
                //TODO: add log here
                return ProviderException.ThrowIfProviderNotFoundByRequest<ICurrencyProvider>(
                    "Provider not found",
                    typeof(T));
            return provider;
        }
        catch (Exception ex)
        {
            //TODO: add log here
            return ProviderException.ThrowIfResolutionFailure<ICurrencyProvider>(
                "Provider resolution failure",
                ex);
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