using AutoMapper;
using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Api.Paths;
using Istu.Navigation.Domain.Models.ExternalRoutes;
using Istu.Navigation.Domain.Services;
using Istu.Navigation.Domain.Services.Buildings;
using Istu.Navigation.Domain.Services.ExternalRoutes;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.Errors;
using Istu.Navigation.Infrastructure.Errors.ExternalRoutesApiErrors;
using Istu.Navigation.Public.Models.ExternalRoutes;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route(ApiRoutes.ExternalRoutes.ExternalRoutesApi)]
public class ExternalRoutesController(
    IBuildingsService buildingsService,
    IExternalRoutesSearcher routesSearcher,
    IMapper mapper) : ControllerBase
{
    
    [HttpPost]
    [Route(ApiRoutes.ExternalRoutes.CreatePart)]
    public async Task<ActionResult<ExternalRouteResponse>> FindRoute([FromBody] ExternalRouteRequest request)
    {
        var getOperation = await buildingsService.GetBuildingCoordinates(request.BuildingId).ConfigureAwait(false);
        if (getOperation.IsFailure)
        {
            var apiError = getOperation.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        var buildingPoint = getOperation.Data;
        var routeOperation = routesSearcher.FindRoute(mapper.Map<ExternalPoint>(request.StartPointDto), buildingPoint,
            request.Type);

        if (routeOperation.IsFailure && routeOperation.ApiError.Urn == ExternalRoutesApiError
                .EndPointOutsideAreaError(buildingPoint.Latitude, buildingPoint.Longitude).Urn)
        {
            var apiError = OperationResult.Failure(CommonErrors.InternalServerError()).ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }
        
        if(routeOperation.IsFailure)
        {
            var apiError = routeOperation.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        return Ok(mapper.Map<ExternalRouteResponse>(routeOperation.Data));
    }
}