using Istu.Navigation.Api.Converters;
using Istu.Navigation.Domain.Services;
using Istu.Navigation.Public.Models;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;
[ApiController]

[Route("api/buildings/routes")]
public class BuildingRoutesController: ControllerBase
{
    private readonly BuildingRoutesService buildingRoutesService;
    private readonly IRoutesConverter routesConverter;

    public BuildingRoutesController(BuildingRoutesService buildingRoutesService, IRoutesConverter routesConverter)
    {
        this.buildingRoutesService = buildingRoutesService;
        this.routesConverter = routesConverter;
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<BuildingRouteResponse>> CreateRoute([FromBody] BuildingRouteRequest request)
    {
        var internalRoute = await buildingRoutesService
            .CreateRoute(request.BuildingId, request.ToId, request.FromId ?? default).ConfigureAwait(false);

        var publicRoute = routesConverter.ConvertToPublicRoute(internalRoute);
        return Ok(publicRoute);
    }

    [HttpGet]
    [Route("{routeId:guid}")]
    public async Task<ActionResult<BuildingRouteResponse>> GetRouteById(Guid routeId)
    {
        var internalRoute = await buildingRoutesService.GetRouteById(routeId).ConfigureAwait(false);

        var publicRoute = routesConverter.ConvertToPublicRoute(internalRoute);
        return Ok(publicRoute);
    }
    
    //Todo: реализовать поиск созданных путей по фильтру 
    /*[HttpGet]
    [Route("{routeId:guid}")]
    public async Task<ActionResult<List<BuildingRouteResponse>>> SearchRoutes(Guid routeId)
    {
        return Ok();
    }*/
}