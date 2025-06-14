using Currency.Common.Pagination;
using Currency.Facades.Contracts.Dtos;

namespace Currency.Facades.Contracts.Responses;

public class GetHistoryResponse
{
    public string Currency { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public IEnumerable<RatesHistoryPartDto> History { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public GetHistoryResponse() {}
    
    public GetHistoryResponse(string currency, DateTime startDate, DateTime endDate, 
        PagedList<RatesHistoryPartDto> history)
    {
        Currency = currency;
        StartDate = startDate;
        EndDate = endDate;
        History = history.Items;
        PageNumber = history.PageNumber;
        PageSize = history.PageSize;
    }
}