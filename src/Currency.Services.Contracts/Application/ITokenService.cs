using Currency.Domain.Login;
using Currency.Domain.Users;

namespace Currency.Services.Contracts.Application;

public interface ITokenService
{
    Tokens GenerateTokens(User user, CancellationToken ct);
    AccessToken GenerateAccessToken(User user, CancellationToken ct);
    Task<RefreshToken> GetRefreshTokenAsync(string refreshToken, CancellationToken ct);
    Task AddRefreshTokenAsync(string refreshToken, string userId, CancellationToken ct);
}