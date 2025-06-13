using Currency.Data.Contracts;
using Currency.Data.Models;
using Currency.Domain.Login;
using Currency.Infrastructure.Contracts.Databases;

namespace Currency.Data.Repositories;

internal class AuthRepository(IRedisContext context) : IAuthRepository
{
    private const string AuthStorage = "auth";
    private const string Prefix = "auth_";

    public async Task AddRefreshToken(RefreshToken refreshToken)
    {
        await context.SetAsync(Prefix + refreshToken.Token, refreshToken, refreshToken.ExpiresAt, AuthStorage);
    }

    public async Task<RefreshToken> GetRefreshTokenAsync(string refreshToken)
    {
        var token = await context.GetAsync<RefreshTokenModel>(Prefix + refreshToken, AuthStorage);
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