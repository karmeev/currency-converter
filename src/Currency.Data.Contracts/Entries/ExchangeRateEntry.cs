namespace Currency.Data.Contracts.Entries;

public class ExchangeRateEntry
{
    public string Provider { get; set; }
    public string Currency { get; set; }
    public decimal Value { get; set; }
    public DateTime Date { get; set; }
}