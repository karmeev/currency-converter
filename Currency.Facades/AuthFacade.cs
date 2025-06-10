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
    ICacheService cacheService,
    ITokenService tokenService): IAuthFacade
{
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var validationResult = validator.Validate(request.Username, request.Password);
        if (!validationResult.IsValid)
        {
            return LoginResponse.Error(validationResult.Message);
        }
        
        var model = new LoginModel(request.Username, request.Password);
        var isUserExist = await userService.CheckUser(model);
        if (!isUserExist)
        {
            return LoginResponse.Error("Incorrect credentials");
        }

        var claims = tokenService.GetClaims(model);
        var accessToken = tokenService.GenerateAccessToken(claims);
        var refreshToken = tokenService.GenerateRefreshToken(model);
        
        await cacheService.InsertNewRefreshToken(model);

        return new LoginResponse(claims, accessToken, refreshToken);
    }
}