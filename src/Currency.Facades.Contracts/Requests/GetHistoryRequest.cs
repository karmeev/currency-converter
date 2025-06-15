namespace Currency.Facades.Contracts.Requests;

public class GetHistoryRequest
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string Currency { get; set; } = "EUR";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}