namespace Currency.Domain.Rates;

public struct ExchangeRatesHistoryPart
{
    public DateTime Date;
    
    public string Currency;
    
    public decimal Value;
}