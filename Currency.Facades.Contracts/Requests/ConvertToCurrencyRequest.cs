namespace Currency.Facades.Contracts.Requests;

public class ConvertToCurrencyRequest
{
    public decimal Amount { get; set; }
    public string FromCurrency { get; set; }
    public string ToCurrency { get; set; }
}