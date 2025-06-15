namespace Currency.Facades.Contracts.Responses;

public class RetrieveLatestRatesResponse(string currentCurrency, 
    DateTime lastDate, 
    Dictionary<string, decimal> rates)
{
    public string CurrentCurrency { get; init; } = currentCurrency;
    public DateTime LastDate { get; init; } = lastDate;
    public Dictionary<string, decimal> Rates { get; init; } = rates;
}