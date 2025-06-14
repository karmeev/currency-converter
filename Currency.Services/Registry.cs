using System.Threading.Channels;
using Autofac;
using Currency.Domain.Rates;
using Currency.Services.Application;
using Currency.Services.Application.Consumers;
using Currency.Services.Contracts.Application;
using Currency.Services.Contracts.Domain;
using Currency.Services.Domain;

namespace Currency.Services;

public static class Registry
{
    public static void RegisterDependencies(ContainerBuilder container)
    {
        RegisterConsumers(container);
        container.RegisterType<UserService>().As<IUserService>();
        container.RegisterType<TokenService>().As<ITokenService>();
        container.RegisterType<ConverterService>().As<IConverterService>();
        container.RegisterType<ExchangeRatesService>().As<IExchangeRatesService>();
    }

    private static void RegisterConsumers(ContainerBuilder container)
    {
        //Consumers
        container.RegisterType<ConsumerService>().As<IConsumerService>().SingleInstance();
        container.RegisterType<ExchangeRatesHistoryConsumer>().InstancePerLifetimeScope();

        //Channels
        container.Register(_ => 
                Channel.CreateBounded<ExchangeRatesHistory>(new BoundedChannelOptions(100)))
            .As<Channel<ExchangeRatesHistory>>()
            .SingleInstance();

        container.Register(c => c.Resolve<Channel<ExchangeRatesHistory>>().Writer)
            .As<ChannelWriter<ExchangeRatesHistory>>()
            .SingleInstance();
    }
}