using Currency.Data.Contracts;
using Currency.Domain.Login;
using Currency.Domain.Users;
using Currency.Infrastructure.Contracts.Auth;
using Currency.Services.Contracts.Application;

namespace Currency.Services.Application;

internal class UserService(
    ISecretHasher secretHasher,
    IUsersRepository usersRepository) : IUserService
{
#nullable enable
    public async Task<User?> TryGetUserAsync(LoginModel model, CancellationToken token)
    {
        var user = await usersRepository.GetUserByUsernameAsync(model, token);
        if (user == null) return null;
        return secretHasher.Verify(model.Password, user.Password) ? user : null;
    }

    public async Task<User?> TryGetUserByIdAsync(string userId, CancellationToken token)
    {
        return await usersRepository.GetUserByIdAsync(userId, token);
    }
}