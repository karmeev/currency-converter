using System.Security.Claims;
using Currency.Domain.Login;
using Currency.Services.Contracts.Application;

namespace Currency.Services.Application;

internal class TokenService: ITokenService
{
    public IEnumerable<Claim> GetClaims(LoginModel model)
    {
        throw new NotImplementedException();
    }

    public string GenerateRefreshToken(LoginModel model)
    {
        throw new NotImplementedException();
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        throw new NotImplementedException();
    }
}