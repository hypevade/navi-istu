using AutoMapper;
using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Api.Filters;
using Istu.Navigation.Api.Paths;
using Istu.Navigation.Domain.Models.ExternalRoutes;
using Istu.Navigation.Domain.Models.Users;
using Istu.Navigation.Domain.Services.Buildings;
using Istu.Navigation.Domain.Services.ExternalRoutes;
using Istu.Navigation.Public.Models.ExternalRoutes;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route(ApiRoutes.ExternalRoutes.ExternalRoutesApi)] [AuthorizationFilter(UserRole.User)]
public class ExternalRoutesController : ControllerBase
{
    private readonly IBuildingsService buildingsService;
    private readonly IExternalRoutesSearcher routesSearcher;
    private readonly IMapper mapper;

    public ExternalRoutesController(IBuildingsService buildingsService,
        IExternalRoutesSearcher routesSearcher,
        IMapper mapper)
    {
        this.buildingsService = buildingsService; this.routesSearcher = routesSearcher; this.mapper = mapper;
    }

    [HttpPost]
    [Route(ApiRoutes.ExternalRoutes.CreatePart)]
    public async Task<ActionResult<ExternalRouteResponse>> FindRoute([FromBody] ExternalRouteRequest request)
    {
        var getOperation = await buildingsService.GetBuildingCoordinatesAsync(request.BuildingId).ConfigureAwait(false);
        if (getOperation.IsFailure)
            return StatusCode(getOperation.ApiError.StatusCode, getOperation.ApiError.ToErrorDto());

        var buildingPoint = getOperation.Data;
        
        var routeOperation = routesSearcher.FindRoute(mapper.Map<ExternalPoint>(request.StartPointDto), buildingPoint,
            request.Type);
        
        return routeOperation.IsFailure
            ? StatusCode(routeOperation.ApiError.StatusCode, routeOperation.ApiError.ToErrorDto())
            : Ok(mapper.Map<ExternalRouteResponse>(routeOperation.Data));
    }
}