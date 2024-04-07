using AutoMapper;
using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Api.Paths;
using Istu.Navigation.Domain.Services;
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
        var getInternalRoute = await buildingRoutesService
            .CreateRoute(request.BuildingId, request.ToId, request.FromId ?? default).ConfigureAwait(false);
        if (getInternalRoute.IsFailure)
        {
            var apiError = getInternalRoute.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }
        
        var publicRoute = mapper.Map<BuildingRouteResponse>(getInternalRoute.Data);
        return Ok(publicRoute);
    }
    
    

    /*[HttpGet]
    [Route("{routeId:guid}")]
    public async Task<ActionResult<BuildingRouteResponse>> GetRouteById(Guid routeId)
    {
        var getInternalRoute = await buildingRoutesService.GetRouteById(routeId).ConfigureAwait(false);
        if (getInternalRoute.IsFailure)
        {
            //TODO: временно написал return NotFound, надо переделать
            //Можно написать метод расширения, для конвертации OperationResult в ActionResult
            //Им будет проще пользоваться
            return NotFound();
        }
        var internalRoute = getInternalRoute.Data;

        var publicRoute = routesConverter.ConvertToPublicRoute(internalRoute);
        return Ok(publicRoute);
    }*/
    
    //Todo: реализовать поиск созданных путей по фильтру 
    /*[HttpGet]
    [Route("{routeId:guid}")]
    public async Task<ActionResult<List<BuildingRouteResponse>>> SearchRoutes(Guid routeId)
    {
        return Ok();
    }*/
}