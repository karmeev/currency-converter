using System.Security.Claims;
using Currency.Domain.Login;
using Currency.Domain.Users;

namespace Currency.Services.Contracts.Application;

public interface ITokenService
{
    (Tokens, IEnumerable<Claim>) GenerateTokens(User user);
    (AccessToken, IEnumerable<Claim>) GenerateAccessToken(User user);
    Task<RefreshToken> GetRefreshTokenAsync(string refreshToken);
    Task AddRefreshTokenAsync(string refreshToken, string userId);
}