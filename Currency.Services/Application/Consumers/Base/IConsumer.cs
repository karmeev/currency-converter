namespace Currency.Services.Application.Consumers.Base;

internal interface IConsumer<in T>
{
    Task Consume(T message, CancellationToken token);
}