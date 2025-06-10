using Currency.Domain.Login;

namespace Currency.Services.Contracts.Application;

public interface ICacheService
{
    Task InsertNewRefreshToken(LoginModel model);
}