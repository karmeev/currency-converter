using Currency.Data.Contracts.Entries;

namespace Currency.Data.Contracts;

public interface IExchangeRatesHistoryRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="token"></param>
    /// <returns>Can return []: if no values found, if Cancellation is requested</returns>
    Task<IEnumerable<ExchangeRateEntry>> GetRateHistoryPagedAsync(string id, int pageNumber, int pageSize, 
        CancellationToken token);
    
    Task AddRateHistory(string id, IEnumerable<ExchangeRateEntry> rates, CancellationToken token);
}