using Autofac;
using Currency.Infrastructure.Contracts.Integrations.Providers;
using Currency.Infrastructure.Contracts.Integrations.Providers.Base;
using Currency.Infrastructure.Contracts.Integrations.Providers.Frankfurter;
using Currency.Infrastructure.Integrations.Providers.Frankfurter;

namespace Currency.Infrastructure.Integrations.Providers;

internal class CurrencyProvidersFactory(ILifetimeScope scope): ICurrencyProvidersFactory
{
    public T TryGetCurrencyProvider<T>() where T : ICurrencyProvider
    {
        try
        {
            var provider = scope.ResolveKeyed<T>(typeof(IFactoryCurrencyProvider<T>));
            return provider;
        }
        catch (Exception ex)
        {
            //TODO: add log here
            return default;
        }
    }
    
    internal static void RegisterProviders(ContainerBuilder container)
    {
        container.RegisterType<FrankfurterProvider>()
            .Keyed<IFrankfurterProvider>(typeof(IFactoryCurrencyProvider<IFrankfurterProvider>))
            .InstancePerLifetimeScope();

        container.RegisterType<CurrencyProvidersFactory>()
            .As<ICurrencyProvidersFactory>()
            .SingleInstance();
    }

    private interface IFactoryCurrencyProvider<T> 
        where T : ICurrencyProvider
    { }
}