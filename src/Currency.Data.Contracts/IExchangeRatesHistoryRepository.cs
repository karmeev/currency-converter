using Currency.Data.Contracts.Entries;
using Currency.Domain.Rates;

namespace Currency.Data.Contracts;

public interface IExchangeRatesHistoryRepository
{
    Task<IEnumerable<ExchangeRatesHistoryPart>> GetRateHistoryPagedAsync(string id, int pageNumber, int pageSize, 
        CancellationToken token);
    
    Task AddRateHistory(string id, IEnumerable<ExchangeRateEntry> rates, CancellationToken token);
}