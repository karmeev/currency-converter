using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Currency.Api.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class CurrencyController : ControllerBase
{
    private const string DefaultCurrency = "EUR";

    [HttpGet]
    public async Task GetExchangeRateHistoryAsync()
    {
    }

    [HttpPost]
    public async Task ConvertCurrencyAsync()
    {
    }

    [HttpGet("latest")]
    public async Task GetLatestExchangeRatesAsync([FromQuery] string currency)
    {
        if (string.IsNullOrEmpty(currency))
            currency = DefaultCurrency;
    }
}