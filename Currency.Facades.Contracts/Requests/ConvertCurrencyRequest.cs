namespace Currency.Facades.Contracts.Requests;

public class ConvertCurrencyRequest
{
    public int Amount { get; set; }
    public string FromCurrency { get; set; }
    public string ToCurrency { get; set; }
}