namespace Currency.Domain.Rates;

public class ExchangeRatesHistory
{
    public string Provider { get; init; }
    public string CurrentCurrency { get; init; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Dictionary<DateTime, Dictionary<string, decimal>> Rates { get; init; } = new();
}