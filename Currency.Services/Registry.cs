using Autofac;
using Currency.Services.Application;
using Currency.Services.Application.Settings;
using Currency.Services.Contracts.Application;
using Currency.Services.Contracts.Domain;
using Currency.Services.Domain;

namespace Currency.Services;

public static class Registry
{
    public static void RegisterDependencies(ContainerBuilder container, ServicesSettings settings)
    {
        container.RegisterInstance(settings).SingleInstance();
        container.RegisterType<UserService>().As<IUserService>();
        container.RegisterType<TokenService>().As<ITokenService>();
        container.RegisterType<ConverterService>().As<IConverterService>();
    }
}