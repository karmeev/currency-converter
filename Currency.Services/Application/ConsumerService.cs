using System.Collections.Concurrent;
using System.Threading.Channels;
using Autofac;
using Currency.Domain.Rates;
using Currency.Services.Application.Consumers;
using Currency.Services.Contracts.Application;

namespace Currency.Services.Application;

internal class ConsumerService(
    ILifetimeScope lifetimeScope,
    Channel<ExchangeRatesHistory> exchangeRatesHistoryChannel): IDisposable, IConsumerService
{
    private readonly CancellationTokenSource _cts = new();
    private readonly SemaphoreSlim _limiter = new(10);
    private readonly ConcurrentQueue<ExchangeRatesHistory> _exchangeRatesQueue = new();
    
    public void Start()
    {
        Task.Run(() => ConsumeExchangeRatesHistoryMessage(_cts.Token));
        for (var i = 0; i < 10; i++)
        {
            Task.Run(() => ConsumeExchangeRatesHistory(_cts.Token));
        }
    }
    
    private async Task ConsumeExchangeRatesHistoryMessage(CancellationToken token)
    {
        await foreach (var message in exchangeRatesHistoryChannel.Reader.ReadAllAsync(token))
        {
            await _limiter.WaitAsync(token);
            _exchangeRatesQueue.Enqueue(message);
        }
    }

    private async Task ConsumeExchangeRatesHistory(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (_exchangeRatesQueue.TryDequeue(out var message))
            {
                await using var scope = lifetimeScope.BeginLifetimeScope();
                var consumer = scope.Resolve<ExchangeRatesHistoryConsumer>();
                await consumer.Consume(message, token);
                _limiter.Release();
            }
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        DisposeQueues();
        _limiter.Dispose();
    }

    private void DisposeQueues()
    {
        exchangeRatesHistoryChannel.Writer.Complete();
        while (_exchangeRatesQueue.TryDequeue(out _)) { }
    }
}