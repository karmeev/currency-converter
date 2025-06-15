using Autofac;
using Currency.Data.Contracts;
using Currency.Data.Repositories;

namespace Currency.Data;

public static class Registry
{
    public static void RegisterDependencies(ContainerBuilder builder)
    {
        builder.RegisterType<UsersRepository>().As<IUsersRepository>();
        builder.RegisterType<AuthRepository>().As<IAuthRepository>();
        builder.RegisterType<ExchangeRatesRepository>().As<IExchangeRatesRepository>();
        builder.RegisterType<ExchangeRatesHistoryRepository>().As<IExchangeRatesHistoryRepository>();
    }
}