using Currency.Services.Contracts.Application;

namespace Currency.Api.BackgroundServices;

public class ConsumersStartupBackgroundService(IConsumerService consumerService): IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        consumerService.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}