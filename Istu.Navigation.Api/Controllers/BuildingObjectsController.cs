using AutoMapper;
using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Api.Paths;
using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Services;
using Istu.Navigation.Public.Models;
using Istu.Navigation.Public.Models.BuildingRoutes;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route(ApiRoutes.BuildingObjects.BuildingsObjectsApi)]

//TODO: Реализовать поиск с фильтром (объединение нескольких методов в один ) GetAll
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
    public async Task<ActionResult<List<FullBuildingObjectDto>>> GetAll([FromQuery] Guid buildingId,
        [FromQuery] PublicObjectType[]? types = null, [FromQuery] int floor = 0, [FromQuery] int skip = 0, [FromQuery] int take = 100)
    {
        if(types.Length == 0)
            return Ok(new List<FullBuildingObjectDto>());
        
        var innerTypes = mapper.Map<BuildingObjectType[]>(types);
        var getObjects = await buildingObjectsService.GetAllByTypeInBuilding(buildingId, innerTypes, skip, take)
            .ConfigureAwait(false);
        if (getObjects.IsFailure)
        {
            var apiError = getObjects.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        var publicObjects = mapper.Map<List<FullBuildingObjectDto>>(getObjects.Data);
        return Ok(publicObjects);
    }
    
    [HttpGet]
    [Route(ApiRoutes.BuildingObjects.GetByIdPart)]
    public async Task<ActionResult<FullBuildingObjectDto>> GetById(Guid objectId)
    {
        var getObject = await buildingObjectsService.GetById(objectId).ConfigureAwait(false);
        if (getObject.IsFailure)
        {
            var apiError = getObject.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        var publicObjects = mapper.Map<List<FullBuildingObjectDto>>(getObject.Data);
        return Ok(publicObjects);
    }
}