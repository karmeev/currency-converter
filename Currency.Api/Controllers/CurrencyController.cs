using Asp.Versioning;
using Currency.Facades.Contracts;
using Currency.Facades.Contracts.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Currency.Api.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class CurrencyController(ICurrencyFacade facade) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetExchangeRateHistoryAsync([FromQuery] GetExchangeRateHistoryRequest request)
    {
        var response = await facade.GetExchangeRatesHistoryAsync(request, HttpContext.RequestAborted);
        return Ok(response);
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestExchangeRatesAsync([FromQuery] string currency = "EUR")
    {
        var response = await facade.RetrieveLatestExchangeRatesAsync(currency, HttpContext.RequestAborted);
        return Ok(response);
    }
    
    [HttpPost("convert")]
    public async Task<IActionResult> ConvertCurrencyAsync([FromQuery] ConvertCurrencyRequest request)
    {
        var response = await facade.ConvertToCurrencyAsync(request, HttpContext.RequestAborted);
        return Ok(response);
    }
}