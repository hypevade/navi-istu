using AutoMapper;
using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Services;
using Istu.Navigation.Public.Models;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route("api/buildings")]
public class BuildingsController : ControllerBase
{
    private readonly IMapper mapper;
    private readonly IBuildingsService buildingsService;

    public BuildingsController(IMapper mapper, IBuildingsService buildingsService)
    {
        this.mapper = mapper;
        this.buildingsService = buildingsService;
    }
    
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> Create([FromBody] CreateBuildingsRequest request)
    {
        if (request.Buildings.Count == 0)
            return Accepted();
        
        var buildings = mapper.Map<List<Building>>(request.Buildings);

        var createOperation = await buildingsService.CreateBuildings(buildings).ConfigureAwait(false);
        if (createOperation.IsFailure)
        {
            var apiError = createOperation.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }
        return Accepted();
    }
    
    [HttpPatch]
    [Route("")]
    public async Task<IActionResult> Update([FromBody] UpdateBuildingsRequest request)
    {
        if (request.Buildings.Count == 0)
            return Accepted();
        
        var buildings = mapper.Map<List<Building>>(request.Buildings);

        var patchBuildings = await buildingsService.PatchBuildings(buildings).ConfigureAwait(false);
        if (patchBuildings.IsFailure)
        {
            var apiError = patchBuildings.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }
        return Accepted();
    }
    
    [HttpDelete]
    [Route("{buildingId}")]
    public async Task<IActionResult> Delete(Guid buildingId)
    {
        var createOperation = await buildingsService.DeleteBuilding(buildingId).ConfigureAwait(false);
        if (createOperation.IsFailure)
        {
            var apiError = createOperation.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }
        
        return Accepted();
    }
}