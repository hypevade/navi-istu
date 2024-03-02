using Istu.Navigation.Api.Converters;
using Istu.Navigation.Domain.Services;
using Istu.Navigation.Public.Models;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;
[ApiController]

[Route("api/routes")]
public class InnerRoutesController: ControllerBase
{
    private readonly InnerRoutesService innerRoutesService;
    private readonly IRoutesConverter routesConverter;

    public InnerRoutesController(InnerRoutesService innerRoutesService, IRoutesConverter routesConverter)
    {
        this.innerRoutesService = innerRoutesService;
        this.routesConverter = routesConverter;
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<BuildingRouteResponse>> CreateRoute([FromBody] BuildingRouteRequest request)
    {
        var internalRoute = await innerRoutesService
            .CreateRoute(request.BuildingId, request.ToId, request.FromId ?? default).ConfigureAwait(false);

        var publicRoute = routesConverter.ConvertToPublicRoute(internalRoute);
        return Ok(publicRoute);
    }

    [HttpGet]
    [Route("{routeId:guid}")]
    public async Task<ActionResult<BuildingRouteResponse>> GetRouteById(Guid routeId)
    {
        return Ok();
    }
    
    //Todo: реализовать поиск созданных путей по фильтру 
    /*[HttpGet]
    [Route("{routeId:guid}")]
    public async Task<ActionResult<List<BuildingRouteResponse>>> SearchRoutes(Guid routeId)
    {
        return Ok();
    }*/
}