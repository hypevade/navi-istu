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

        return createOperation.IsSuccess
            ? Ok(new CreateBuildingResponse { BuildingId = createOperation.Data })
            : StatusCode(createOperation.ApiError.StatusCode, createOperation.ApiError.ToErrorDto());
    }
    
    [HttpPatch]
    [Route(ApiRoutes.Buildings.UpdateBuildingPart)]
    public async Task<IActionResult> Update([FromBody] UpdateBuildingRequest request)
    {
        var update = await buildingsService
            .Patch(request.Id, request.UpdatedTitle, request.UpdatedLatitude, request.UpdatedLongitude,
                request.UpdatedDescription)
            .ConfigureAwait(false);
        return update.IsSuccess
            ? Accepted()
            : StatusCode(update.ApiError.StatusCode, update.ApiError.ToErrorDto());
    }

    [HttpDelete]
    [Route(ApiRoutes.Buildings.DeleteBuildingPart)]
    public async Task<IActionResult> Delete(Guid buildingId)
    {
        var delete = await buildingsService.Delete(buildingId).ConfigureAwait(false);
        return delete.IsSuccess 
            ? Accepted() 
            : StatusCode(delete.ApiError.StatusCode, delete.ApiError.ToErrorDto());
    }

    [HttpGet]
    [Route(ApiRoutes.Buildings.GetBuildingPart)]
    public async Task<ActionResult<BuildingDto>> GetById(Guid buildingId)
    {
        var getBuilding = await buildingsService.GetById(buildingId).ConfigureAwait(false);
        return getBuilding.IsSuccess
            ? Ok(mapper.Map<BuildingDto>(getBuilding.Data))
            : StatusCode(getBuilding.ApiError.StatusCode, getBuilding.ApiError.ToErrorDto());
    }

    [HttpGet]
    [Route(ApiRoutes.Buildings.GetAllBuildingsPart)]
    public async Task<ActionResult<List<BuildingDto>>> GetAllByFilter([FromQuery] BuildingFilter filter)
    {
        var getBuilding = await buildingsService.GetAllByFilter(filter).ConfigureAwait(false);
        return getBuilding.IsSuccess
            ? Ok(mapper.Map<List<BuildingDto>>(getBuilding.Data))
            : StatusCode(getBuilding.ApiError.StatusCode, getBuilding.ApiError.ToErrorDto());
    }

    [HttpPost]
    [Route(ApiRoutes.Buildings.CreateFloorPart)]
    public async Task<ActionResult<CreateFloorResponse>> CreateFloor(Guid buildingId,
        [FromBody] CreateFloorRequest request)
    {
        var createOperation = await floorsService.CreateFloor(buildingId, request.ImageLink, request.FloorNumber)
            .ConfigureAwait(false);
        return createOperation.IsSuccess
            ? Ok(new CreateFloorResponse { FloorId = createOperation.Data })
            : StatusCode(createOperation.ApiError.StatusCode, createOperation.ApiError.ToErrorDto());
    }

    [HttpGet]
    [Route(ApiRoutes.Buildings.GetFloorPart)]
    public async Task<ActionResult<FloorDto>> GetFloor(Guid buildingId, int floorNumber)
    {
        var floor = await floorsBuilder.GetFloor(buildingId, floorNumber)
            .ConfigureAwait(false);
        return floor.IsSuccess
            ? Ok(mapper.Map<FloorDto>(floor.Data))
            : StatusCode(floor.ApiError.StatusCode, floor.ApiError.ToErrorDto());
    }

    [HttpDelete]
    [Route(ApiRoutes.Buildings.DeleteFloorPart)]
    public async Task<IActionResult> DeleteFloor(Guid buildingId, int floorNumber)
    {
        var deleteFloor = await floorsService.DeleteFloor(buildingId, floorNumber).ConfigureAwait(false);
        return deleteFloor.IsSuccess
            ? Accepted()
            : StatusCode(deleteFloor.ApiError.StatusCode, deleteFloor.ApiError.ToErrorDto());
    }

    [HttpGet]
    [Route(ApiRoutes.Buildings.GetFloorsPart)]
    public async Task<ActionResult<List<FloorDto>>> GetFloorsByBuilding(Guid buildingId)
    {
        var floors = await floorsService.GetFloorInfosByBuilding(buildingId).ConfigureAwait(false);
        return floors.IsSuccess
            ? Ok(mapper.Map<List<FloorDto>>(floors.Data))
            : StatusCode(floors.ApiError.StatusCode, floors.ApiError.ToErrorDto());
    }
}

