using Asp.Versioning;
using Currency.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Currency.Api.Controllers;

[ApiController]
[Authorize(Roles = nameof(UserRole.Admin))]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class StatusController: ControllerBase
{
    [HttpGet("health")]
    public IActionResult GetStatus()
    {
        return Ok(new
        {
            Application = "Currency API",
            Version = "2.0",
            Status = "Healthy"
        });
    }
}