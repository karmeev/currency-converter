using System.Threading.Channels;
using Autofac;
using Currency.Services.Contracts.Application;

namespace Currency.Services.Application;

public class PublisherService(ILifetimeScope scope) : IPublisherService
{
    public async Task Publish<T>(T message, CancellationToken token)
    {
        if (token.IsCancellationRequested) return;
        
        var channel = scope.Resolve<ChannelWriter<T>>();
        await channel.WriteAsync(message, CancellationToken.None);
    }
}