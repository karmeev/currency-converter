namespace Currency.Facades.Contracts.Dtos;

public class RatesHistoryPartDto
{
    public DateTime Date { get; init; }
    public string Currency { get; init; }
    public decimal Value { get; init; }
}