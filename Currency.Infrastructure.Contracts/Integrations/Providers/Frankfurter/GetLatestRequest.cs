using Currency.Infrastructure.Contracts.Integrations.Providers.Base.Requests;

namespace Currency.Infrastructure.Contracts.Integrations.Providers.Frankfurter;

public record GetLatestRequest(string Currency) : IGetLatestRequest;