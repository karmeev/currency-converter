using Currency.Common.Pagination;
using Currency.Domain.Rates;

namespace Currency.Facades.Contracts.Responses;

public class GetHistoryResponse
{
    public string Currency { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public IEnumerable<ExchangeRatesHistoryPart> History { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public GetHistoryResponse() {}
    
    public GetHistoryResponse(string currency, DateTime startDate, DateTime endDate, 
        PagedList<ExchangeRatesHistoryPart> history)
    {
        Currency = currency;
        StartDate = startDate;
        EndDate = endDate;
        History = history.Items;
        PageNumber = history.PageNumber;
        PageSize = history.PageSize;
    }
}