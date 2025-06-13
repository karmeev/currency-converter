using Currency.Services.Contracts.Domain;

namespace Currency.Facades;

//design concept
internal class CurrencyFacade(
    IConverterService converterService,
    IExchangeRatesService exchangeRatesService)
{
    public async Task RetrieveLatestExchangeRates()
    {
        var currency = "USD";

        //validation here, then
        await exchangeRatesService.GetLatestExchangeRates(currency);
    }

    public async Task ConvertToCurrency()
    {
        var amount = 10.021m;
        var currency = "USD";
        var currency2 = "EUR";

        //validation here, then
        await converterService.ConvertToCurrency(amount, currency, currency2);
    }

    public async Task GetExchangeRatesHistory()
    {
    }
}