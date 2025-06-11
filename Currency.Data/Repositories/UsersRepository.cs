using System.Collections.Immutable;
using Currency.Data.Contracts;
using Currency.Domain.Login;
using Currency.Domain.Users;

namespace Currency.Data.Repositories;

internal class UsersRepository : IUsersRepository
{
    internal ImmutableList<User> Users { get; set; } = ImmutableList<User>.Empty;

    public async ValueTask<User> GetUserByUsernameAsync(LoginModel model)
    {
        var user = Users.Find(u => u.Username == model.Username);
        return user;
    }

    public async ValueTask<User> GetUserByIdAsync(string id)
    {
        var user = Users.Find(u => u.Id == id);
        return user;
    }
}