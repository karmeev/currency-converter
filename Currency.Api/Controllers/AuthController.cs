using Asp.Versioning;
using Currency.Facades.Contracts;
using Currency.Facades.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Currency.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController(IAuthFacade facade) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        var response = await facade.LoginAsync(request, HttpContext.RequestAborted);
        if (!response.Success)
            return BadRequest(response.ErrorMessage);

        return Ok(new
        {
            response.AccessToken,
            response.RefreshToken,
            response.ExpiresAt
        });
    }

    [HttpPost("refreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var response = await facade.RefreshTokenAsync(request.Token, HttpContext.RequestAborted);
        if (!response.Success)
            return BadRequest(response.ErrorMessage);

        return Ok(new
        {
            response.AccessToken,
            response.RefreshToken,
            response.ExpiresAt
        });
    }
}