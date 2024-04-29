using AutoMapper;
using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Api.Paths;
using Istu.Navigation.Domain.Services;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Public.Models.Buildings;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route(ApiRoutes.Buildings.BuildingsApi)]
public class BuildingsController : ControllerBase
{
    private readonly IMapper mapper;
    private readonly IBuildingsService buildingsService;
    private readonly IFloorsService floorsService;
    private readonly IFloorsBuilder floorsBuilder;

    public BuildingsController(IMapper mapper,
        IBuildingsService buildingsService,
        IFloorsService floorsService,
        IFloorsBuilder floorsBuilder)
    {
        this.mapper = mapper;
        this.buildingsService = buildingsService;
        this.floorsService = floorsService;
        this.floorsBuilder = floorsBuilder;
    }

    [HttpPost]
    [Route(ApiRoutes.Buildings.CreateBuildingPart)]
    public async Task<ActionResult<CreateBuildingResponse>> Create([FromBody] CreateBuildingRequest request)
    {
        var createOperation = await buildingsService
            .Create(request.Title, request.Latitude, request.Longitude, request.Description).ConfigureAwait(false);

        if (createOperation.IsFailure)
            return StatusCode(createOperation.ApiError.StatusCode, createOperation.ApiError.ToErrorDto());

        return Ok(new CreateBuildingResponse { BuildingId = createOperation.Data });
    }
    
    //Todo: add patch method
    [HttpPatch]
    [Route(ApiRoutes.Buildings.UpdateBuildingPart)]
    public async Task<IActionResult> Update([FromBody] UpdateBuildingRequest request)
    {
        return Ok();
        /*var building = mapper.Map<Building>(request.Building);

        var patchBuildings = await buildingsService.Patch(building).ConfigureAwait(false);
        if (patchBuildings.IsFailure)
        {
            var apiError = patchBuildings.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        return Accepted();*/
    }

    [HttpDelete]
    [Route(ApiRoutes.Buildings.DeleteBuildingPart)]
    public async Task<IActionResult> Delete(Guid buildingId)
    {
        var createOperation = await buildingsService.Delete(buildingId).ConfigureAwait(false);
        if (createOperation.IsFailure)
        {
            var apiError = createOperation.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        return Accepted();
    }

    [HttpGet]
    [Route(ApiRoutes.Buildings.GetBuildingPart)]
    public async Task<ActionResult<BuildingDto>> GetById(Guid buildingId)
    {
        var getBuilding = await buildingsService.GetById(buildingId).ConfigureAwait(false);
        if (getBuilding.IsFailure)
        {
            var apiError = getBuilding.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        var buildingDto = mapper.Map<BuildingDto>(getBuilding.Data);
        return buildingDto;
    }

    [HttpGet]
    [Route(ApiRoutes.Buildings.GetAllBuildingsPart)]
    public async Task<ActionResult<List<BuildingDto>>> GetAllByFilter([FromQuery] BuildingFilter filter)
    {
        var getBuilding = await buildingsService.GetAllByFilter(filter).ConfigureAwait(false);
        if (getBuilding.IsFailure)
        {
            var apiError = getBuilding.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        return Ok(mapper.Map<List<BuildingDto>>(getBuilding.Data));
    }

    [HttpPost]
    [Route("{buildingId}/floors")]
    public async Task<ActionResult<CreateFloorResponse>> CreateFloor(Guid buildingId,
        [FromBody] CreateFloorRequest request)
    {
        var createOperation = await floorsService.CreateFloor(buildingId, request.ImageLink, request.FloorNumber)
            .ConfigureAwait(false);
        if (createOperation.IsFailure)
            return StatusCode(createOperation.ApiError.StatusCode, createOperation.ApiError.ToErrorDto());
        return Ok(new CreateFloorResponse { FloorId = createOperation.Data });
    }
    
    [HttpGet]
    [Route("{buildingId}/floors/{floorNumber}")]
    public async Task<ActionResult<FloorDto>> GetFloor(Guid buildingId, int floorNumber)
    {
        var floor = await floorsBuilder.GetFloor(buildingId, floorNumber)
            .ConfigureAwait(false);
        if (floor.IsFailure)
            return StatusCode(floor.ApiError.StatusCode, floor.ApiError.ToErrorDto());
        return Ok(mapper.Map<FloorDto>(floor.Data));
    }

    [HttpDelete]
    [Route("{buildingId}/floors/{floorNumber}")]
    public async Task<IActionResult> DeleteFloor(Guid buildingId, int floorNumber)
    {
        var createOperation = await floorsService.DeleteFloor(buildingId, floorNumber).ConfigureAwait(false);
        return createOperation.IsSuccess
            ? Accepted()
            : StatusCode(createOperation.ApiError.StatusCode, createOperation.ApiError.ToErrorDto());
    }
}

