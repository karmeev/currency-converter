using Currency.Infrastructure.Contracts.Integrations.Providers.Frankfurter;

namespace Currency.Infrastructure.Integrations.Providers.Frankfurter;

internal class FrankfurterProvider(IFrankfurterClient client): IFrankfurterProvider
{
   //my method, logic etc
   
   public void Dispose()
   {
      client.Dispose();
   }
}