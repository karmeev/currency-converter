using Currency.Infrastructure.Contracts.Integrations.Providers.Base;

namespace Currency.Infrastructure.Contracts.Integrations.Providers;

#nullable enable
public interface ICurrencyProvidersFactory
{
    T? TryGetCurrencyProvider<T>() where T : ICurrencyProvider;
}