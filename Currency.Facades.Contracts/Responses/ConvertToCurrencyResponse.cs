namespace Currency.Facades.Contracts.Responses;

public class ConvertToCurrencyResponse(decimal amount, string currency)
{
    public decimal Amount { get; init; } = amount;
    public string Currency { get; init; } = currency;
}