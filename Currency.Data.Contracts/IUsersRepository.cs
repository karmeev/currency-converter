using Currency.Domain.Login;
using Currency.Domain.Users;

namespace Currency.Data.Contracts;

public interface IUsersRepository
{
    ValueTask<User> GetUserByUsernameAsync(LoginModel model);
    ValueTask<User> GetUserByIdAsync(string id);
}