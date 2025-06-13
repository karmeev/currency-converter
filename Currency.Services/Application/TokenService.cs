using System.Security.Claims;
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
    public (Tokens, IEnumerable<Claim>) GenerateTokens(User user)
    {
        var (accessToken, claims) = GenerateAccessToken(user);
        var refreshToken = tokenGenerator.CreateRefreshToken(user.Username);

        var tokens = new Tokens
        {
            AccessToken = accessToken.Token,
            ExpiresAt = accessToken.ExpiresAt,
            RefreshToken = refreshToken
        };
        return (tokens, claims);
    }

    public (AccessToken, IEnumerable<Claim>) GenerateAccessToken(User user)
    {
        var claims = tokenGenerator.BuildClaims(user.Id,
            user.Username, user.Role);
        var accessToken = tokenGenerator.CreateAccessToken(claims);

        return (accessToken, claims);
    }

    public async Task<RefreshToken> GetRefreshTokenAsync(string refreshToken)
    {
        return await authRepository.GetRefreshTokenAsync(refreshToken);
    }

    public async Task AddRefreshTokenAsync(string refreshToken, string userId)
    {
        var token = new RefreshToken
        {
            Verified = true,
            UserId = userId,
            Token = refreshToken,
            ExpirationDate = DateTime.UtcNow.AddDays(settings.RefreshTokenTtlInDays),
            ExpiresAt = TimeSpan.FromDays(settings.RefreshTokenTtlInDays)
        };
        await authRepository.AddRefreshToken(token);
    }
}