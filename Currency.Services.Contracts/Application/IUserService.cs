using Currency.Domain.Login;
using Currency.Domain.Users;

namespace Currency.Services.Contracts.Application;

#nullable enable
public interface IUserService
{
    Task<User?> TryGetUserAsync(LoginModel model);
    Task<User?> TryGetUserByIdAsync(string userId);
}