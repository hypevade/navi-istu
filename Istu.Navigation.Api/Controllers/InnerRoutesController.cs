using Istu.Navigation.Api.Converters;
using Istu.Navigation.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;
[ApiController]
[Route("api/routes")]
public class InnerRoutesController: ControllerBase
{
    private InnerRoutesService innerRoutesService;
    private IRoutesConverter routesConverter;

    public InnerRoutesController(InnerRoutesService innerRoutesService, IRoutesConverter routesConverter)
    {
        this.innerRoutesService = innerRoutesService;
        this.routesConverter = routesConverter;
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateRoute(Guid buildingId, Guid toId, Guid fromId = default)
    {
        var internalRoute = await innerRoutesService.CreateRoute(buildingId, toId, fromId).ConfigureAwait(false);
        var publicRoute = routesConverter.ConvertToPublicRoute(internalRoute);
        return Ok(publicRoute);
    }
    
}