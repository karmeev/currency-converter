using System.Collections.Immutable;
using Autofac;
using Currency.Data.Contracts;
using Currency.Data.Repositories;
using Currency.Domain.Users;

namespace Currency.Data;

public static class Registry
{
    public static void RegisterDependencies(ContainerBuilder builder, IEnumerable<User> users)
    {
        builder.RegisterType<UsersRepository>().As<IUsersRepository>()
            .OnActivated(e => { e.Instance.Users = users.ToImmutableList(); })
            .PropertiesAutowired();

        builder.RegisterType<AuthRepository>().As<IAuthRepository>();
        builder.RegisterType<ExchangeRatesHistoryRepository>().As<IExchangeRatesHistoryRepository>();
    }
}