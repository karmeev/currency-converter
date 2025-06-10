using Currency.Facades.Contracts.Requests;
using Currency.Facades.Contracts.Responses;

namespace Currency.Facades.Contracts;

public interface IAuthFacade
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}