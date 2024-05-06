using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Api.Paths;
using Istu.Navigation.Domain.Services;
using Istu.Navigation.Domain.Services.Buildings;
using Istu.Navigation.Public.Models.ExternalRoutes;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route(ApiRoutes.ExternalRoutes.ExternalRoutesApi)]
public class ExternalRoutesController(IBuildingsService service) : ControllerBase
{
    [HttpPost]
    [Route(ApiRoutes.ExternalRoutes.CreatePart)]
    public async Task<ActionResult<ExternalRouteResponse>> FindRoute([FromBody] ExternalRouteRequest request)
    {
        var getBuildingOperation = await service.GetById(request.BuildingId).ConfigureAwait(false);
        if (getBuildingOperation.IsFailure)
        {
            var apiError = getBuildingOperation.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }
        
        return Ok();
    }
}