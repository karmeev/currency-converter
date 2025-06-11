using Currency.Domain.Login;

namespace Currency.Data.Contracts;

public interface IAuthRepository
{
    Task AddRefreshToken(RefreshToken refreshToken);
    Task<RefreshToken> GetRefreshTokenAsync(string refreshToken);
}