using System.Collections.Concurrent;
using System.Threading.Channels;
using Autofac;
using Currency.Domain.Operations;
using Currency.Domain.Rates;
using Currency.Services.Application.Consumers.Base;
using Currency.Services.Application.Settings;
using Currency.Services.Contracts.Application;
using Microsoft.Extensions.Logging;

namespace Currency.Services.Application;

internal class ConsumerService(
    ILifetimeScope lifetimeScope,
    ServicesSettings settings,
    ILogger<ConsumerService> logger,
    Channel<ExchangeRatesHistory> exchangeRatesHistoryChannel,
    Channel<CurrencyConversion> currencyConversionChannel,
    Channel<ExchangeRates> exchangeRatesChannel): IDisposable, IConsumerService
{
    private bool _disposed;
    private SemaphoreSlim _limiter = new(10);
    private readonly CancellationTokenSource _cts = new();
    private readonly ConcurrentQueue<ExchangeRatesHistory> _historyQueue = new();
    private readonly ConcurrentQueue<CurrencyConversion> _currencyConversionQueue = new();
    private readonly ConcurrentQueue<ExchangeRates> _exchangeRatesQueue = new();
    
    public void Start()
    {
        try
        {
            _limiter = new SemaphoreSlim(settings.Worker.Bandwidth);
            var historyWorkers = settings.Worker.ExchangeRatesHistoryWorkers;
            var conversionWorkers = settings.Worker.CurrencyConversionWorkers;
            var ratesWorkers = settings.Worker.ExchangeRatesWorkers;

            if (historyWorkers > 0)
            {
                StartReceiver(exchangeRatesHistoryChannel, _historyQueue, _cts.Token);
                for (var i = 0; i < historyWorkers; i++)
                {
                    StartConsumer(_historyQueue, _cts.Token);
                }
            }

            if (conversionWorkers > 0)
            {
                StartReceiver(currencyConversionChannel, _currencyConversionQueue, _cts.Token);
                for (var i = 0; i < conversionWorkers; i++)
                {
                    StartConsumer(_currencyConversionQueue, _cts.Token);
                }
            }

            if (ratesWorkers > 0)
            {
                StartReceiver(exchangeRatesChannel, _exchangeRatesQueue, _cts.Token);
                for (var i = 0; i < ratesWorkers; i++)
                {
                    StartConsumer(_exchangeRatesQueue, _cts.Token);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception was encountered during startup workers");
        }
    }
    
    private Task StartReceiver<T>(Channel<T> channel, ConcurrentQueue<T> queue, CancellationToken token)
    {
        return Task.Run(async () =>
        {
            await foreach (var message in channel.Reader.ReadAllAsync(token))
            {
                await _limiter.WaitAsync(token);
                queue.Enqueue(message);
            }
        }, token);
    }
    
    private Task StartConsumer<T>(ConcurrentQueue<T> queue, CancellationToken token)
    {
        return Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                if (queue.IsEmpty)
                {
                    await Task.Delay(settings.Worker.ConsumeDelayInMilliseconds, token);
                    continue;
                }
                
                if (queue.TryDequeue(out var message))
                {
                    await HandleAsync(async () =>
                    {
                        await using var scope = lifetimeScope.BeginLifetimeScope();
                        var consumer = scope.Resolve<IConsumer<T>>();
                        await consumer.Consume(message, token);
                    });
                }
            }
        }, token);
    }

    private async Task HandleAsync(Func<Task> consume)
    {
        try
        {
            await consume();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled consumer error: {message}", ex.Message);
        }
        finally
        {
            _limiter.Release();
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _cts.Cancel();
        DisposeQueues();
        _limiter.Dispose();
    }

    private void DisposeQueues()
    {
        exchangeRatesChannel.Writer.Complete();
        currencyConversionChannel.Writer.Complete();
        exchangeRatesHistoryChannel.Writer.Complete();
        while (_historyQueue.TryDequeue(out _)) { }
        while (_currencyConversionQueue.TryDequeue(out _)) { }
        while (_exchangeRatesQueue.TryDequeue(out _)) { }
    }
}