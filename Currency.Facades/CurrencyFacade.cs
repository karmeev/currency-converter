using Currency.Services.Contracts.Domain;

namespace Currency.Facades;

//design concept
internal class CurrencyFacade(
    IConverterService converterService,
    IExchangeRatesService exchangeRatesService)
{
    public async Task RetrieveLatestExchangeRates()
    {
        string currency = "USD";
        
        //validation here, then
        await exchangeRatesService.GetLatestExchangeRates(currency);
    }

    public async Task ConvertToCurrency()
    {
        decimal amount = 10.021m;
        string currency = "USD";
        string currency2 = "EUR";
        
        //validation here, then
        await converterService.ConvertToCurrency(amount, currency, currency2);
    }

    public async Task GetExchangeRatesHistory()
    {
        
    }
}