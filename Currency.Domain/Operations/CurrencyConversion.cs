namespace Currency.Domain.Operations;

public class CurrencyConversion
{
    public string Provider { get; init; }
    public decimal Amount { get; init; }
    public string FromCurrency { get; set; }
    public string ToCurrency { get; init; }
}