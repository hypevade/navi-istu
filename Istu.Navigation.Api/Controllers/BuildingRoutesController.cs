using AutoMapper;
using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Api.Filters;
using Istu.Navigation.Api.Paths;
using Istu.Navigation.Domain.Models.Users;
using Istu.Navigation.Domain.Services.BuildingRoutes;
using Istu.Navigation.Domain.Services.Buildings;
using Istu.Navigation.Public.Models.BuildingRoutes;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route(ApiRoutes.BuildingRoutes.BuildingsRoutesApi)]
[AuthorizationFilter(UserRole.User)]
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
        var getOperation = await buildingRoutesService.CreateRouteAsync(request.ToId, request.FromId ?? default)
            .ConfigureAwait(false);
        
        return getOperation.IsFailure
            ? StatusCode(getOperation.ApiError.StatusCode, getOperation.ApiError.ToErrorDto())
            : Ok(mapper.Map<BuildingRouteResponse>(getOperation.Data));
    }
}