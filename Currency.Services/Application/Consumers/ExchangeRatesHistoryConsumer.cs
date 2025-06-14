using Currency.Data.Contracts;
using Currency.Data.Contracts.Entries;
using Currency.Domain.Rates;

namespace Currency.Services.Application.Consumers;

internal class ExchangeRatesHistoryConsumer(IExchangeRatesHistoryRepository repository)
{
    public async Task Consume(ExchangeRatesHistory message, CancellationToken token)
    {
        var key = $"{message.Provider}:{message.CurrentCurrency}:{message.StartDate:yyyyMMddHHmmss}:{message.EndDate:yyyyMMddHHmmss}";
    
        var entries = new List<ExchangeRateEntry>();

        foreach (var (date, rates) in message.Rates)
        {
            entries.AddRange(rates.Select(kvp => new ExchangeRateEntry
            {
                Provider = message.Provider,
                Currency = kvp.Key,
                Value = kvp.Value,
                Date = date
            }));
        }

        await repository.AddRateHistory(key, entries, token);
        entries.Clear();
    }
}