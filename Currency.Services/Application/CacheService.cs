using Currency.Domain.Login;
using Currency.Services.Contracts;
using Currency.Services.Contracts.Application;

namespace Currency.Services.Application;

internal class CacheService: ICacheService
{
    public Task InsertNewRefreshToken(LoginModel model)
    {
        throw new NotImplementedException();
    }
}