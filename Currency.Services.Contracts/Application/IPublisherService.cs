namespace Currency.Services.Contracts.Application;

public interface IPublisherService
{
    Task Publish<T>(T message, CancellationToken token);
}