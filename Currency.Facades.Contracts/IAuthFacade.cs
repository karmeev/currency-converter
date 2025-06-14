using Currency.Facades.Contracts.Requests;
using Currency.Facades.Contracts.Responses;

namespace Currency.Facades.Contracts;

public interface IAuthFacade
{
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct);
    Task<AuthResponse> RefreshTokenAsync(string token, CancellationToken ct);
}