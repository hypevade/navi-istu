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
[Route(ApiRoutes.BuildingObjects.BuildingsObjectsApi)]
public class BuildingObjectsController(
    IBuildingObjectsService objectsService,
    IMapper mapper)
    : ControllerBase
{
    [HttpPatch]
    [Route(ApiRoutes.BuildingObjects.UpdatePart)]
    [AuthorizationFilter(UserRole.Admin)]
    
    public Task<ActionResult<CreateBuildingObjectResponse>> Update([FromBody] UpdateObjectRequest request)
    {
        throw new NotImplementedException();
    }

    
    [HttpPost]
    [Route(ApiRoutes.BuildingObjects.CreatePart)]
    [AuthorizationFilter(UserRole.Admin)]
    public async Task<ActionResult<CreateBuildingObjectResponse>> Create([FromBody] CreateBuildingObjectRequest request)
    {
        var buildingObject = request.ToBuildingObject(Guid.NewGuid());

        var createOperation = await objectsService.CreateAsync(buildingObject);

        if (createOperation.IsFailure)
        {
            var apiError = createOperation.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }
        
        return Ok(new CreateBuildingObjectResponse() { BuildingObjectId = createOperation.Data });
    }

    [HttpGet]
    [Route(ApiRoutes.BuildingObjects.GetAllPart)]
    [AuthorizationFilter(UserRole.User)]
    public async Task<ActionResult<List<BuildingObjectDto>>> GetAll([FromQuery] BuildingObjectFilter filter)
    {
        var getObjects = await objectsService.GetAllByFilterAsync(filter)
            .ConfigureAwait(false);
        if (getObjects.IsFailure)
        {
            var apiError = getObjects.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        var publicObjects = mapper.Map<List<BuildingObjectDto>>(getObjects.Data);
        return Ok(publicObjects);
    }

    [HttpGet]
    [Route(ApiRoutes.BuildingObjects.GetByIdPart)]
    [AuthorizationFilter(UserRole.User)]
    public async Task<ActionResult<BuildingObjectDto>> GetById(Guid objectId)
    {
        var getObject = await objectsService.GetByIdAsync(objectId).ConfigureAwait(false);
        if (getObject.IsFailure)
        {
            var apiError = getObject.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        var publicObjects = mapper.Map<BuildingObjectDto>(getObject.Data);
        return Ok(publicObjects);
    }
    
    [HttpDelete]
    [Route(ApiRoutes.BuildingObjects.DeletePart)]
    [AuthorizationFilter(UserRole.Admin)]
    public async Task<ActionResult<BuildingObjectDto>> Delete(Guid objectId)
    {
        var getObject = await objectsService.DeleteAsync([objectId]).ConfigureAwait(false);
        if (getObject.IsFailure)
        {
            var apiError = getObject.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        return NoContent();
    }
}