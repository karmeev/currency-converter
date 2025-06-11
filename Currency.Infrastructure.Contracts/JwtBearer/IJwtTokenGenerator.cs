using System.Security.Claims;
using Currency.Domain.Login;
using Currency.Domain.Users;

namespace Currency.Infrastructure.Contracts.JwtBearer;

public interface IJwtTokenGenerator
{
    IEnumerable<Claim> BuildClaims(string identifier, string username, UserRole role);
    
    AccessToken CreateAccessToken(IEnumerable<Claim> claims);
    
    string CreateRefreshToken(string username);
}