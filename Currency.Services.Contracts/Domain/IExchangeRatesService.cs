namespace Currency.Services.Contracts.Domain;

public interface IExchangeRatesService
{
    Task GetLatestExchangeRates(string currency);
}