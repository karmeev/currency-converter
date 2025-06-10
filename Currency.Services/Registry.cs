using Autofac;
using Currency.Services.Application;
using Currency.Services.Contracts.Application;

namespace Currency.Services;

public static class Registry
{
    public static void RegisterDependencies(ContainerBuilder container)
    {
        container.RegisterType<UserService>().As<IUserService>();
        container.RegisterType<CacheService>().As<ICacheService>();
        container.RegisterType<TokenService>().As<ITokenService>();
    }
}