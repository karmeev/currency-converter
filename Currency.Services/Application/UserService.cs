using Currency.Domain.Login;
using Currency.Services.Contracts;
using Currency.Services.Contracts.Application;

namespace Currency.Services.Application;

internal class UserService: IUserService
{
    public Task<bool> CheckUser(LoginModel model)
    {
        throw new NotImplementedException();
    }
}