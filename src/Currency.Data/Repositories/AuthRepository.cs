using Currency.Data.Contracts;
using Currency.Data.Models;
using Currency.Domain.Login;
using Currency.Infrastructure.Contracts.Databases;
using Currency.Infrastructure.Contracts.Databases.Base;
using Currency.Infrastructure.Contracts.Databases.Redis;

namespace Currency.Data.Repositories;

internal class AuthRepository(IRedisContext context) : IAuthRepository
{
    private static string Prefix => EntityPrefix.AuthPrefix;
    
    public async Task AddRefreshToken(RefreshToken refreshToken, CancellationToken ct)
    {
        if (ct.IsCancellationRequested) return;
        
        await context.SetAsync($"{Prefix}:{refreshToken.Token}", refreshToken, refreshToken.ExpiresAt);
    }

    public async Task<RefreshToken> GetRefreshTokenAsync(string refreshToken, CancellationToken ct)
    {
        var token = await context.TryGetAsync<RefreshTokenModel>($"{Prefix}:{refreshToken}");
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