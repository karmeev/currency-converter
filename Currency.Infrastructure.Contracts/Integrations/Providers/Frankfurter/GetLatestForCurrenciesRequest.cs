using Currency.Infrastructure.Contracts.Integrations.Providers.Base.Requests;

namespace Currency.Infrastructure.Contracts.Integrations.Providers.Frankfurter;

public record GetLatestForCurrenciesRequest(
    string Currency,
    string[] Symbols) : IGetLatestForCurrenciesRequest;