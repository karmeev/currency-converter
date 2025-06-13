using Currency.Infrastructure.Contracts.Integrations.Providers.Base;
using Currency.Infrastructure.Contracts.Integrations.Providers.Base.Requests;

namespace Currency.Infrastructure.Contracts.Integrations.Providers;

#nullable enable
public interface ICurrencyProvidersFactory
{
    ICurrencyProvider TryGetCurrencyProvider<T>(T request) where T : ICurrencyProviderRequest;
}