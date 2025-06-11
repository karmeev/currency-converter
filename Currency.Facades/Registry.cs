using Autofac;
using Currency.Facades.Contracts;
using Currency.Facades.Validators;

namespace Currency.Facades;

public static class Registry
{
    public static void RegisterDependencies(ContainerBuilder container)
    {
        container.RegisterType<AuthFacade>().As<IAuthFacade>();
        container.RegisterType<AuthValidator>().As<IAuthValidator>().SingleInstance();
    }
}