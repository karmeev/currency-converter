using Currency.Domain.Login;
using Currency.Facades.Contracts;
using Currency.Facades.Contracts.Requests;
using Currency.Facades.Contracts.Responses;
using Currency.Facades.Validators;
using Currency.Services.Contracts.Application;

namespace Currency.Facades;

internal class AuthFacade(
    IAuthValidator validator,
    IUserService userService,
    ITokenService tokenService) : IAuthFacade
{
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var validationResult = validator.Validate(request.Username, request.Password);
        if (!validationResult.IsValid) return AuthResponse.Error(validationResult.Message);

        var model = new LoginModel(request.Username, request.Password);
        var user = await userService.TryGetUserAsync(model);
        if (user is null) return AuthResponse.Error("Incorrect credentials");

        var (tokenModel, claims) = tokenService.GenerateTokens(user);
        await tokenService.AddRefreshTokenAsync(tokenModel.RefreshToken, user.Id);

        return new AuthResponse(claims, tokenModel.AccessToken, tokenModel.RefreshToken,
            tokenModel.ExpiresAt);
    }

    public async Task<AuthResponse> RefreshTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token)) return AuthResponse.Error("Invalid refresh token");
        var refreshToken = await tokenService.GetRefreshTokenAsync(token);
        if (!refreshToken.Verified) return AuthResponse.Error("Refresh token is not verified");

        var user = await userService.TryGetUserByIdAsync(refreshToken.UserId);
        if (user is null) return AuthResponse.Error("User not found");

        var (accessToken, claims) = tokenService.GenerateAccessToken(user);

        return new AuthResponse(claims, accessToken.Token, token, accessToken.ExpiresAt);
    }
}