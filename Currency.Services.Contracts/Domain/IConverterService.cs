namespace Currency.Services.Contracts.Domain;

public interface IConverterService
{
    Task ConvertToCurrency(decimal amount, string currentCurrency, string newCurrency);
}