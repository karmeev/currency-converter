using Currency.Domain.Login;
using Currency.Domain.Users;

namespace Currency.Data.Contracts;

public interface IUsersRepository
{
    Task<User> GetUserByUsernameAsync(LoginModel model);
    Task<User> GetUserByIdAsync(string id);
}