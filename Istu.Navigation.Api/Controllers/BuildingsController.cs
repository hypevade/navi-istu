using AutoMapper;
using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Api.Filters;
using Istu.Navigation.Api.Paths;
using Istu.Navigation.Domain.Models.Users;
using Istu.Navigation.Domain.Services.Buildings;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Public.Models.Buildings;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;

[ApiController] [Route(ApiRoutes.BuildingsRoutes.BuildingsApi)]
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
        this.mapper = mapper; this.buildingsService = buildingsService; this.floorsService = floorsService;
        this.floorsBuilder = floorsBuilder;
    }

    [HttpPost]
    [Route(ApiRoutes.BuildingsRoutes.CreateBuildingPart)] [AuthorizationFilter(UserRole.Admin)]
    public async Task<ActionResult<CreateBuildingResponse>> Create([FromBody] CreateBuildingRequest request)
    {
        var createOperation = await buildingsService
            .CreateAsync(request.Title, request.ExternalPosition.Latitude, request.ExternalPosition.Longitude, request.Address,
                keywords: request.Keywords, description: request.Description).ConfigureAwait(false);

        return createOperation.IsSuccess
            ? Ok(new CreateBuildingResponse { BuildingId = createOperation.Data })
            : StatusCode(createOperation.ApiError.StatusCode, createOperation.ApiError.ToErrorDto());
    }
    
    [HttpPatch]
    [Route(ApiRoutes.BuildingsRoutes.UpdateBuildingPart)] [AuthorizationFilter(UserRole.Admin)]
    public async Task<IActionResult> Update([FromBody] UpdateBuildingRequest request)
    {
        var update = await buildingsService
            .PatchAsync(request.Id, request.UpdatedTitle, request.UpdatedLatitude, request.UpdatedLongitude,
                request.UpdatedAddress,
                request.UpdatedDescription).ConfigureAwait(false);
        
        return update.IsSuccess
            ? NoContent()
            : StatusCode(update.ApiError.StatusCode, update.ApiError.ToErrorDto());
    }

    [HttpDelete]
    [Route(ApiRoutes.BuildingsRoutes.DeleteBuildingPart)] [AuthorizationFilter(UserRole.Admin)]
    public async Task<IActionResult> Delete(Guid buildingId)
    {
        var delete = await buildingsService.DeleteAsync(buildingId).ConfigureAwait(false);
        return delete.IsSuccess 
            ? NoContent() 
            : StatusCode(delete.ApiError.StatusCode, delete.ApiError.ToErrorDto());
    }

    [HttpGet]
    [Route(ApiRoutes.BuildingsRoutes.GetBuildingPart)] [AuthorizationFilter(UserRole.User)]
    public async Task<ActionResult<BuildingDto>> GetById(Guid buildingId)
    {
        var getOperation = await buildingsService.GetByIdAsync(buildingId).ConfigureAwait(false);
        return getOperation.IsSuccess
            ? Ok(mapper.Map<BuildingDto>(getOperation.Data))
            : StatusCode(getOperation.ApiError.StatusCode, getOperation.ApiError.ToErrorDto());
    }

    [HttpGet]
    [Route(ApiRoutes.BuildingsRoutes.GetAllBuildingsPart)] [AuthorizationFilter(UserRole.User)]
    public async Task<ActionResult<List<BuildingDto>>> GetAllByFilter([FromQuery] BuildingFilter filter)
    {
        var getOperation = await buildingsService.GetAllByFilterAsync(filter).ConfigureAwait(false);
        
        return getOperation.IsSuccess
            ? Ok(mapper.Map<List<BuildingDto>>(getOperation.Data))
            : StatusCode(getOperation.ApiError.StatusCode, getOperation.ApiError.ToErrorDto());
    }

    [HttpPost]
    [Route(ApiRoutes.BuildingsRoutes.CreateFloorPart)] [AuthorizationFilter(UserRole.Admin)]
    public async Task<ActionResult<CreateFloorResponse>> CreateFloor(Guid buildingId,
        [FromBody] CreateFloorRequest request)
    {
        var createOperation = await floorsService.CreateFloor(buildingId, request.FloorNumber)
            .ConfigureAwait(false);
        return createOperation.IsSuccess
            ? Ok(new CreateFloorResponse { FloorId = createOperation.Data })
            : StatusCode(createOperation.ApiError.StatusCode, createOperation.ApiError.ToErrorDto());
    }

    [HttpGet]
    [Route(ApiRoutes.BuildingsRoutes.GetFloorPart)] [AuthorizationFilter(UserRole.User)]
    public async Task<ActionResult<FloorDto>> GetFloor(Guid buildingId, int floorNumber)
    {
        var getOperation = await floorsBuilder.GetFloor(buildingId, floorNumber)
            .ConfigureAwait(false);
        
        return getOperation.IsSuccess
            ? Ok(mapper.Map<FloorDto>(getOperation.Data))
            : StatusCode(getOperation.ApiError.StatusCode, getOperation.ApiError.ToErrorDto());
    }

    [HttpDelete]
    [Route(ApiRoutes.BuildingsRoutes.DeleteFloorPart)]
    [AuthorizationFilter(UserRole.Admin)]
    public async Task<IActionResult> DeleteFloor(Guid buildingId, int floorNumber)
    {
        var deleteOperation = await floorsService.DeleteFloor(buildingId, floorNumber).ConfigureAwait(false);
        return deleteOperation.IsSuccess
            ? NoContent()
            : StatusCode(deleteOperation.ApiError.StatusCode, deleteOperation.ApiError.ToErrorDto());
    }

    [HttpGet]
    [Route(ApiRoutes.BuildingsRoutes.GetFloorsPart)] [AuthorizationFilter(UserRole.User)]
    public async Task<ActionResult<List<FloorDto>>> GetFloorsByBuilding(Guid buildingId)
    {
        var getOperation = await floorsBuilder.GetFloorsByBuilding(buildingId).ConfigureAwait(false);
        if (getOperation == null) throw new ArgumentNullException(nameof(getOperation));
        return getOperation.IsSuccess
            ? Ok(mapper.Map<List<FloorDto>>(getOperation.Data))
            : StatusCode(getOperation.ApiError.StatusCode, getOperation.ApiError.ToErrorDto());
    }
}

