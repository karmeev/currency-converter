using System.Threading.Channels;
using Autofac;
using Currency.Services.Contracts.Application;

namespace Currency.Services.Application;

public class PublisherService(ILifetimeScope scope) : IPublisherService
{
    public async Task Publish<T>(T message, CancellationToken token)
    {
        if (token.IsCancellationRequested) return;

        if (scope.TryResolve<ChannelWriter<T>>(out var channel))
        {
            await channel.WriteAsync(message, CancellationToken.None);
        }
    }
}