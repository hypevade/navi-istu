using AutoMapper;
using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Api.Paths;
using Istu.Navigation.Domain.Services;
using Istu.Navigation.Domain.Services.Buildings;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Public.Models;
using Istu.Navigation.Public.Models.BuildingObjects;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route(ApiRoutes.BuildingObjects.BuildingsObjectsApi)]

//Добавить методы Update и Delete для объектов и delete для ребер

public class BuildingObjectsController : ControllerBase
{
    private readonly IBuildingObjectsService buildingObjectsService;
    private readonly IEdgesService edgesService;
    private readonly IMapper mapper;

    public BuildingObjectsController(IBuildingObjectsService buildingObjectsService, IMapper mapper, IEdgesService edgesService)
    {
        this.buildingObjectsService = buildingObjectsService;
        this.mapper = mapper;
        this.edgesService = edgesService;
    }
    
    [HttpPatch]
    [Route(ApiRoutes.BuildingObjects.UpdatePart)]
    
    public async Task<ActionResult<CreateBuildingObjectResponse>> Update([FromBody] UpdateObjectRequest request)
    {
        throw new NotImplementedException();
    }

    
    [HttpPost]
    [Route(ApiRoutes.BuildingObjects.CreatePart)]
    public async Task<ActionResult<CreateBuildingObjectResponse>> Create([FromBody] CreateBuildingObjectRequest request)
    {
        var buildingObject = request.ToBuildingObject(Guid.NewGuid());

        var createOperation = await buildingObjectsService.CreateRange(buildingObject);

        if (createOperation.IsFailure)
        {
            var apiError = createOperation.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        return Ok(new CreateBuildingObjectResponse() { BuildingObjectId = createOperation.Data });
    }

    [HttpGet]
    [Route(ApiRoutes.BuildingObjects.GetAllPart)]
    public async Task<ActionResult<List<BuildingObjectDto>>> GetAll([FromQuery] BuildingObjectFilter filter)
    {
        var getObjects = await buildingObjectsService.GetAllByFilter(filter)
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
    public async Task<ActionResult<BuildingObjectDto>> GetById(Guid objectId)
    {
        var getObject = await buildingObjectsService.GetById(objectId).ConfigureAwait(false);
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
    public async Task<ActionResult<BuildingObjectDto>> Delete(Guid objectId)
    {
        var getObject = await buildingObjectsService.Delete([objectId]).ConfigureAwait(false);
        if (getObject.IsFailure)
        {
            var apiError = getObject.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        return NoContent();
    }
}