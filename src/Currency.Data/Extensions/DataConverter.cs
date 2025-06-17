using Currency.Data.Contracts.Entries;
using Currency.Domain.Rates;

namespace Currency.Data.Extensions;

public static class DataConverter
{
    public static IEnumerable<ExchangeRatesHistoryPart> ToPartOfHistory(IEnumerable<ExchangeRateEntry> entry)
    {
        return entry.Select(x => new ExchangeRatesHistoryPart
        {
            Currency = x.Currency,
            Date = x.Date,
            Value = x.Value,
        });
    }
}