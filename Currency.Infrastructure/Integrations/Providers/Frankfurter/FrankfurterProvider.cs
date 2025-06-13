using Currency.Domain.Rates;
using Currency.Infrastructure.Contracts.Integrations.Providers.Base.Requests;
using Currency.Infrastructure.Contracts.Integrations.Providers.Frankfurter;

namespace Currency.Infrastructure.Integrations.Providers.Frankfurter;

internal class FrankfurterProvider(IFrankfurterClient client): IDisposable, IFrankfurterProvider
{
   public Task<ExchangeRates> GetLatestAsync(IGetLatestRequest request, CancellationToken token = default)
   {
      throw new NotImplementedException();
   }

   public Task<ExchangeRates> GetLatestForCurrenciesAsync(IGetLatestForCurrenciesRequest request, CancellationToken token = default)
   {
      throw new NotImplementedException();
   }

   public Task<ExchangeRatesHistory> GetHistoryAsync(IGetHistoryRequest request, CancellationToken token = default)
   {
      throw new NotImplementedException();
   }
   
   public void Dispose()
   {
      client.Dispose();
   }
}