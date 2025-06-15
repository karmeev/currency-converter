using Currency.Domain.Login;

namespace Currency.Data.Contracts;

public interface IAuthRepository
{
    Task AddRefreshToken(RefreshToken refreshToken, CancellationToken ct);
    Task<RefreshToken> GetRefreshTokenAsync(string refreshToken, CancellationToken ct);
}