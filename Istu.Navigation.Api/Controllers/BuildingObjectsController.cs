using AutoMapper;
using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Api.Filters;
using Istu.Navigation.Api.Paths;
using Istu.Navigation.Domain.Models.Users;
using Istu.Navigation.Domain.Services.Buildings;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Public.Models;
using Istu.Navigation.Public.Models.BuildingObjects;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route(ApiRoutes.BuildingObjectsRoutes.BuildingsObjectsApi)]
public class BuildingObjectsController : ControllerBase
{
    private readonly IBuildingObjectsService objectsService;
    private readonly IMapper mapper;

    public BuildingObjectsController(IBuildingObjectsService objectsService,
        IMapper mapper)
    {
        this.objectsService = objectsService;
        this.mapper = mapper;
    }

    [HttpPost]
    [Route(ApiRoutes.BuildingObjectsRoutes.CreatePart)]
    [AuthorizationFilter(UserRole.Admin)]
    public async Task<ActionResult<CreateBuildingObjectResponse>> Create([FromBody] CreateBuildingObjectRequest request)
    {
        var buildingObject = request.ToBuildingObject(Guid.NewGuid());

        var createOperation = await objectsService.CreateAsync(buildingObject);

        if (createOperation.IsFailure)
            return StatusCode(createOperation.ApiError.StatusCode, createOperation.ApiError.ToErrorDto());

        return Ok(new CreateBuildingObjectResponse { BuildingObjectId = createOperation.Data });
    }

    [HttpPatch]
    [Route(ApiRoutes.BuildingObjectsRoutes.UpdatePart)]
    [AuthorizationFilter(UserRole.Admin)]
    public async Task<ActionResult<CreateBuildingObjectResponse>> Update([FromBody] UpdateObjectRequest request)
    {
        var updateOperation = await objectsService.PatchAsync(request.BuildingObjectId,
            request.UpdatedTitle,
            request.UpdatedDescription,
            request.UpdatedType,
            request.UpdatedPosition?.X,
            request.UpdatedPosition?.Y).ConfigureAwait(false);
        if (updateOperation.IsFailure)
            return StatusCode(updateOperation.ApiError.StatusCode, updateOperation.ApiError.ToErrorDto());

        return NoContent();
    }

    [HttpDelete]
    [Route(ApiRoutes.BuildingObjectsRoutes.DeletePart)]
    [AuthorizationFilter(UserRole.Admin)]
    public async Task<ActionResult<BuildingObjectDto>> Delete(Guid objectId)
    {
        var deleteOperation = await objectsService.DeleteAsync([objectId]).ConfigureAwait(false);
        if (deleteOperation.IsFailure)
            return StatusCode(deleteOperation.ApiError.StatusCode, deleteOperation.ApiError.ToErrorDto());
        
        return NoContent();
    }

    [HttpGet]
    [Route(ApiRoutes.BuildingObjectsRoutes.GetAllPart)]
    [AuthorizationFilter(UserRole.User)]
    public async Task<ActionResult<List<BuildingObjectDto>>> GetAll([FromQuery] BuildingObjectFilter filter)
    {
        var getOperation = await objectsService.GetAllByFilterAsync(filter)
            .ConfigureAwait(false);
        if (getOperation.IsFailure)
            return StatusCode(getOperation.ApiError.StatusCode, getOperation.ApiError.ToErrorDto());

        var publicObjects = mapper.Map<List<BuildingObjectDto>>(getOperation.Data);
        return Ok(publicObjects);
    }

    [HttpGet]
    [Route(ApiRoutes.BuildingObjectsRoutes.GetByIdPart)]
    [AuthorizationFilter(UserRole.User)]
    public async Task<ActionResult<BuildingObjectDto>> GetById(Guid objectId)
    {
        var getOperation = await objectsService.GetByIdAsync(objectId).ConfigureAwait(false);
        if (getOperation.IsFailure)
            return StatusCode(getOperation.ApiError.StatusCode, getOperation.ApiError.ToErrorDto());

        var publicObjects = mapper.Map<BuildingObjectDto>(getOperation.Data);
        return Ok(publicObjects);
    }
}