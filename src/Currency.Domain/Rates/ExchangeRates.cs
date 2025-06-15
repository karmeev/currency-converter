namespace Currency.Domain.Rates;

public class ExchangeRates
{
    public string Provider { get; set; }
    public string CurrentCurrency { get; set; }
    public DateTime LastDate { get; set; }
    public Dictionary<string, decimal> Rates { get; set; }
}