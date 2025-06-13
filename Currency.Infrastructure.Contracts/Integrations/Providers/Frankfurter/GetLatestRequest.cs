using Currency.Infrastructure.Contracts.Integrations.Providers.Base.Requests;

namespace Currency.Infrastructure.Contracts.Integrations.Providers.Frankfurter;

public class GetLatestRequest(string Currency) : IGetLatestRequest;