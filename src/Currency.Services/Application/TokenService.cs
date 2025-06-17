using Currency.Data.Contracts;
using Currency.Domain.Login;
using Currency.Domain.Users;
using Currency.Infrastructure.Contracts.JwtBearer;
using Currency.Services.Application.Settings;
using Currency.Services.Contracts.Application;

namespace Currency.Services.Application;

internal class TokenService(
    ServicesSettings settings,
    IJwtTokenGenerator tokenGenerator,
    IAuthRepository authRepository) : ITokenService
{
    public Tokens GenerateTokens(User user, CancellationToken ct)
    {
        var accessToken = GenerateAccessToken(user, ct);
        var refreshToken = tokenGenerator.CreateRefreshToken(user.Username);

        var tokens = new Tokens
        {
            AccessToken = accessToken.Token,
            ExpiresAt = accessToken.ExpiresAt,
            RefreshToken = refreshToken
        };
        return tokens;
    }

    public AccessToken GenerateAccessToken(User user, CancellationToken ct)
    {
        var claims = tokenGenerator.BuildClaims(user.Id,
            user.Username, user.Role);
        var accessToken = tokenGenerator.CreateAccessToken(claims);

        return accessToken;
    }

    public async Task<RefreshToken> GetRefreshTokenAsync(string refreshToken, CancellationToken ct)
    {
        return await authRepository.GetRefreshTokenAsync(refreshToken, ct);
    }

    public async Task AddRefreshTokenAsync(string refreshToken, string userId, CancellationToken ct)
    {
        var token = new RefreshToken
        {
            Verified = true,
            UserId = userId,
            Token = refreshToken,
            ExpirationDate = DateTime.UtcNow.AddDays(settings.RefreshTokenTtlInDays),
            ExpiresAt = TimeSpan.FromDays(settings.RefreshTokenTtlInDays)
        };
        await authRepository.AddRefreshToken(token, ct);
    }
}