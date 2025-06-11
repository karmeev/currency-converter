using Currency.Infrastructure.Contracts.Integrations.Providers.Frankfurter.Responses;

namespace Currency.Infrastructure.Integrations.Frankfurter;

public class FrankfurterProvider
{
    public async Task<GetLatestRatesResponse> GetLatestRatesAsync(string @base)
    {
        return new GetLatestRatesResponse();
    }
}