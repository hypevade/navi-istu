using Istu.Navigation.Api.Paths;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route(ApiRoutes.Test)]
public class TestController : ControllerBase
{
    // GET: /Test/Ping
    [HttpGet("Ping")]
    public IActionResult Ping()
    {
        return Ok("API is up and running!");
    }
}