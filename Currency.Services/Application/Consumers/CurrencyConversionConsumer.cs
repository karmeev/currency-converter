using Currency.Data.Contracts;
using Currency.Domain.Operations;
using Currency.Services.Application.Consumers.Base;

namespace Currency.Services.Application.Consumers;

internal class CurrencyConversionConsumer(IExchangeRatesRepository repository) : IConsumer<CurrencyConversion>
{
    public async Task Consume(CurrencyConversion message, CancellationToken token)
    {
        var id = $"{message.Provider}:{message.FromCurrency}:{message.ToCurrency}";
        await repository.AddConversionResultAsync(id, message, token);
    }
}