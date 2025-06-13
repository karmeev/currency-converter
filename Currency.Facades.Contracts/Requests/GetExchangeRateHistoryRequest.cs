namespace Currency.Facades.Contracts.Requests;

public class GetExchangeRateHistoryRequest
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string Currency { get; set; } = "EUR";
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}