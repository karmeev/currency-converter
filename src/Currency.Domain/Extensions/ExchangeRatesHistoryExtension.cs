using Currency.Domain.Rates;

namespace Currency.Domain.Extensions;

public static class ExchangeRatesHistoryExtension
{
    public static IEnumerable<ExchangeRatesHistoryPart> ToPartOfHistory(this ExchangeRatesHistory history)
    {
        return history.Rates.SelectMany(rateByDate => 
            rateByDate.Value.Select(currencyRate => new ExchangeRatesHistoryPart
            {
                Date = rateByDate.Key,
                Currency = currencyRate.Key,
                Value = currencyRate.Value
            }));
    }
}