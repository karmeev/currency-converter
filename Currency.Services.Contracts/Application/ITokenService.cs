using System.Security.Claims;
using Currency.Domain.Login;

namespace Currency.Services.Contracts.Application;

public interface ITokenService
{
    IEnumerable<Claim> GetClaims(LoginModel model);
    string GenerateRefreshToken(LoginModel model);
    string GenerateAccessToken(IEnumerable<Claim> claims);
}