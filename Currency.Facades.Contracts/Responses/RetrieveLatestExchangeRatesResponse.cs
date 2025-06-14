namespace Currency.Facades.Contracts.Responses;

public class RetrieveLatestExchangeRatesResponse
{
    public string CurrentCurrency { get; set; }
    public DateTime LastDate { get; set; }
    public Dictionary<string, decimal> Rates { get; set; }
}