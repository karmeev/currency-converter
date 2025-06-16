using System.Collections.Concurrent;
using System.Threading.Channels;
using Autofac;
using Currency.Domain.Operations;
using Currency.Domain.Rates;
using Currency.Services.Application.Consumers.Base;
using Currency.Services.Contracts.Application;
using Microsoft.Extensions.Logging;

namespace Currency.Services.Application;

internal class ConsumerService(
    ILifetimeScope lifetimeScope,
    Channel<ExchangeRatesHistory> exchangeRatesHistoryChannel,
    Channel<CurrencyConversion> currencyConversionChannel,
    Channel<ExchangeRates> exchangeRatesChannel): IDisposable, IConsumerService
{
    private readonly CancellationTokenSource _cts = new();
    private readonly SemaphoreSlim _limiter = new(10);
    private readonly ConcurrentQueue<ExchangeRatesHistory> _historyQueue = new();
    private readonly ConcurrentQueue<CurrencyConversion> _currencyConversionQueue = new();
    private readonly ConcurrentQueue<ExchangeRates> _exchangeRatesQueue = new();
    
    public void Start()
    {
        Task.Run(() => ReceiveHistoryMessage(_cts.Token));
        Task.Run(() => ReceiveCurrencyConversionMessage(_cts.Token));
        Task.Run(() => ReceiveExchangeRatesMessage(_cts.Token));
        
        for (var i = 0; i < 2; i++)
        {
            Task.Run(() => ConsumeHistory(_cts.Token));
        }
        
        for (var i = 0; i < 2; i++)
        {
            Task.Run(() => ConsumeCurrencyConversion(_cts.Token));
        }
        
        for (var i = 0; i < 2; i++)
        {
            Task.Run(() => ConsumeExchangeRates(_cts.Token));
        }
    }
    
    private async Task ReceiveHistoryMessage(CancellationToken token)
    {
        await foreach (var message in exchangeRatesHistoryChannel.Reader.ReadAllAsync(token))
        {
            await _limiter.WaitAsync(token);
            _historyQueue.Enqueue(message);
        }
    }
    
    private async Task ReceiveCurrencyConversionMessage(CancellationToken token)
    {
        await foreach (var message in currencyConversionChannel.Reader.ReadAllAsync(token))
        {
            await _limiter.WaitAsync(token);
            _currencyConversionQueue.Enqueue(message);
        }
    }
    
    private async Task ReceiveExchangeRatesMessage(CancellationToken token)
    {
        await foreach (var message in exchangeRatesChannel.Reader.ReadAllAsync(token))
        {
            await _limiter.WaitAsync(token);
            _exchangeRatesQueue.Enqueue(message);
        }
    }

    private async Task ConsumeHistory(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (_historyQueue.TryDequeue(out var message))
            {
                await HandleAsync(async () =>
                {
                    await using var scope = lifetimeScope.BeginLifetimeScope();
                    var consumer = scope.Resolve<IConsumer<ExchangeRatesHistory>>();
                    await consumer.Consume(message, token);
                });
            }
        }
    }
    
    private async Task ConsumeCurrencyConversion(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (_currencyConversionQueue.TryDequeue(out var message))
            {
                await HandleAsync(async () =>
                {
                    await using var scope = lifetimeScope.BeginLifetimeScope();
                    var consumer = scope.Resolve<IConsumer<CurrencyConversion>>();
                    await consumer.Consume(message, token);
                });
            }
        }
    }
    
    private async Task ConsumeExchangeRates(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (_exchangeRatesQueue.TryDequeue(out var message))
            {
                await HandleAsync(async () =>
                {
                    await using var scope = lifetimeScope.BeginLifetimeScope();
                    var consumer = scope.Resolve<IConsumer<ExchangeRates>>();
                    await consumer.Consume(message, token);
                });
            }
        }
    }

    private async Task HandleAsync(Func<Task> consume)
    {
        try
        {
            await consume();
        }
        catch (Exception ex)
        {
            var logger = lifetimeScope.Resolve<ILogger<ConsumerService>>();
            logger.LogError(ex, "Unhandled consumer error: {message}", ex.Message);
        }
        finally
        {
            _limiter.Release();
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
        exchangeRatesChannel.Writer.Complete();
        currencyConversionChannel.Writer.Complete();
        exchangeRatesHistoryChannel.Writer.Complete();
        while (_historyQueue.TryDequeue(out _)) { }
        while (_currencyConversionQueue.TryDequeue(out _)) { }
        while (_exchangeRatesQueue.TryDequeue(out _)) { }
    }
}