using Currency.Infrastructure.Contracts.Integrations.Providers.Base;
using Currency.Infrastructure.Contracts.Integrations.Providers.Base.Exceptions;
using Currency.Infrastructure.Contracts.Integrations.Providers.Base.Requests;

namespace Currency.Infrastructure.Contracts.Integrations.Providers;

#nullable enable
public interface ICurrencyProvidersFactory
{
    /// <summary>
    /// Resolves a currency provider instance based on the specified request type.
    /// </summary>
    /// <typeparam name="T">The type of the currency provider request.</typeparam>
    /// <param name="request">The request used to determine which provider to resolve.</param>
    /// <returns>
    /// An instance of <see cref="ICurrencyProvider"/> mapped to the request type.
    /// </returns>
    /// <exception cref="ProviderException">
    /// Thrown when the provider is not found in the mapping or when resolution via the DI container fails.
    /// </exception>
    ICurrencyProvider GetCurrencyProvider<T>(T request) where T : ICurrencyProviderRequest;
}