namespace Currency.Domain.Rates;

public struct ExchangeRatesHistoryPart
{
    public DateTime Date { get; set; }
    public string Currency { get; set; }
    public decimal Value { get; set; }
}