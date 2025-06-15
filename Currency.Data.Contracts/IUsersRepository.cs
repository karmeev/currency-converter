using Currency.Domain.Login;
using Currency.Domain.Users;

namespace Currency.Data.Contracts;

public interface IUsersRepository
{
    Task<User> GetUserByUsernameAsync(LoginModel model, CancellationToken token);
    Task<User> GetUserByIdAsync(string id, CancellationToken token);
}