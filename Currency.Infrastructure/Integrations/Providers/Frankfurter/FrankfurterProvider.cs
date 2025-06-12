using Currency.Domain.Rates;
using Currency.Infrastructure.Contracts.Integrations.Providers.Frankfurter;

namespace Currency.Infrastructure.Integrations.Providers.Frankfurter;

internal class FrankfurterProvider(IFrankfurterClient client): IDisposable, IFrankfurterProvider
{
   public Task<ExchangeRates> GetLatestAsync(CancellationToken token = default)
   {
      throw new NotImplementedException();
   }

   public Task<ExchangeRates> GetLatestForCurrenciesAsync(CancellationToken token = default)
   {
      throw new NotImplementedException();
   }

   public Task<ExchangeRatesHistory> GetHistoryAsync(CancellationToken token = default)
   {
      throw new NotImplementedException();
   }
   
   public void Dispose()
   {
      client.Dispose();
   }
}