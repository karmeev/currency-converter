using Currency.Data.Contracts;
using Currency.Data.Contracts.Entries;
using Currency.Domain.Rates;
using Currency.Services.Application.Consumers.Base;

namespace Currency.Services.Application.Consumers;

internal class ExchangeRatesConsumer(
    IExchangeRatesRepository exchangeRatesRepository,
    IExchangeRatesHistoryRepository historyRepository) : IConsumer<ExchangeRatesHistory>, IConsumer<ExchangeRates>
{
    public async Task Consume(ExchangeRatesHistory message, CancellationToken token)
    {
        var id = $"{message.Provider}:{message.CurrentCurrency}:{message.StartDate:yyyyMMddHHmmss}:{message.EndDate:yyyyMMddHHmmss}";
    
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

        await historyRepository.AddRateHistory(id, entries, token);
        entries.Clear();
    }

    public async Task Consume(ExchangeRates message, CancellationToken token)
    {
        var id = $"{message.Provider}:{message.CurrentCurrency}";
        await exchangeRatesRepository.AddExchangeRates(id, message, token);
    }
}