using AutoMapper;
using Istu.Navigation.Public.Models;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route("api/buildings")]
public class BuildingsController : ControllerBase
{
    private readonly IMapper mapper;

    public BuildingsController(IMapper mapper)
    {
        this.mapper = mapper;
    }
    
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateBuilding([FromBody] CreateBuildingsRequest request)
    {
        if (request.Buildings.Count == 0)
            return Accepted();

        throw new Exception();

    }
}