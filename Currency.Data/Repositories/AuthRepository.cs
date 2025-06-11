using Currency.Data.Contracts;
using Currency.Data.Models;
using Currency.Domain.Login;
using Currency.Infrastructure.Contracts.Databases;

namespace Currency.Data.Repositories;

internal class AuthRepository(IRedisContext context) : IAuthRepository
{
    private const string AuthStorage = "auth";

    public async Task AddRefreshToken(RefreshToken refreshToken)
    {
        await context.SetAsync(refreshToken.Token, refreshToken, refreshToken.ExpiresAt, AuthStorage);
    }

    public async Task<RefreshToken> GetRefreshTokenAsync(string refreshToken)
    {
        var token = await context.GetAsync<RefreshTokenModel>(refreshToken, AuthStorage);
        if (token == null)
            return new RefreshToken();

        return new RefreshToken
        {
            Verified = token.Verified,
            Token = refreshToken,
            ExpiresAt = token.ExpiresAt,
            ExpirationDate = token.ExpirationDate,
            UserId = token.UserId
        };
    }
}