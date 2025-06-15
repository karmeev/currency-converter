using Currency.Data.Contracts.Entries;
using Currency.Domain.Rates;
using Currency.Facades.Contracts.Dtos;

namespace Currency.Facades.Converters;

public static class DtoConverter
{
    public static IEnumerable<RatesHistoryPartDto> ConvertToRatesHistoryPartDto(IEnumerable<ExchangeRateEntry> entry)
    {
        return entry.Select(x => new RatesHistoryPartDto
        {
            Currency = x.Currency,
            Date = x.Date,
            Value = x.Value,
        });
    }
    
    public static IEnumerable<RatesHistoryPartDto> ConvertToRatesHistoryPartDto(ExchangeRatesHistory history)
    {
        return history.Rates.SelectMany(rateByDate => 
            rateByDate.Value.Select(currencyRate => new RatesHistoryPartDto
            {
                Date = rateByDate.Key,
                Currency = currencyRate.Key,
                Value = currencyRate.Value
            }));
    }
}