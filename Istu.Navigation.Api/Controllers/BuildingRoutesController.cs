using AutoMapper;
using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Api.Paths;
using Istu.Navigation.Domain.Services.Buildings;
using Istu.Navigation.Public.Models.BuildingRoutes;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route(ApiRoutes.BuildingRoutes.BuildingsRoutesApi)]
public class BuildingRoutesController: ControllerBase
{
    private readonly IBuildingRoutesService buildingRoutesService;
    private readonly IMapper mapper;

    public BuildingRoutesController(IBuildingRoutesService buildingRoutesService, IMapper mapper)
    {
        this.buildingRoutesService = buildingRoutesService;
        this.mapper = mapper;
    }

    [HttpPost]
    [Route(ApiRoutes.BuildingRoutes.CreatePart)]
    public async Task<ActionResult<BuildingRouteResponse>> CreateRoute([FromBody] BuildingRouteRequest request)
    {
        var getInternalRoute = await buildingRoutesService.CreateRoute(request.ToId, request.FromId ?? default)
            .ConfigureAwait(false);
        if (getInternalRoute.IsFailure)
        {
            var apiError = getInternalRoute.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }
        
        var publicRoute = mapper.Map<BuildingRouteResponse>(getInternalRoute.Data);
        return Ok(publicRoute);
    }
}