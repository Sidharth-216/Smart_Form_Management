using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
public sealed class HealthController : ControllerBase
{
    [HttpGet("/api/health")]
    public IActionResult Get() => Ok(new { status = "healthy" });
}
