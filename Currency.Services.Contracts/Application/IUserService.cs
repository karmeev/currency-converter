using Currency.Domain.Login;

namespace Currency.Services.Contracts.Application;

public interface IUserService
{
    Task<bool> CheckUser(LoginModel model);
}