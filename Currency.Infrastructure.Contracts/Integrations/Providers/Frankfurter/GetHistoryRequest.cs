using Currency.Infrastructure.Contracts.Integrations.Providers.Base.Requests;

namespace Currency.Infrastructure.Contracts.Integrations.Providers.Frankfurter;

public record GetHistoryRequest(
    string Currency,
    DateOnly Start,
    DateOnly End) : IGetHistoryRequest;