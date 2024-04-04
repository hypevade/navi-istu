using AutoMapper;
using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Services;
using Istu.Navigation.Public.Models;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route("api/buildingObjects")]
public class BuildingObjectsController : ControllerBase
{
    private readonly IBuildingObjectsService buildingObjectsService;
    private readonly IMapper mapper;

    public BuildingObjectsController(IBuildingObjectsService buildingObjectsService, IMapper mapper)
    {
        this.buildingObjectsService = buildingObjectsService;
        this.mapper = mapper;
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create([FromBody] CreateBuildingObjectsRequest request)
    {
        if (request.BuildingObjects.Count == 0)
            return Accepted();
        var createOperation = await buildingObjectsService
            .Create(mapper.Map<List<BuildingObject>>(request.BuildingObjects)).ConfigureAwait(false);
        
        if (!createOperation.IsFailure) return Accepted();
        
        var apiError = createOperation.ApiError;
        return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
    }

    [HttpGet]
    [Route("{buildingId}/objectsByTypes")]
    public async Task<ActionResult<List<FullBuildingObjectDto>>> GetAllByTypeInBuilding(Guid buildingId, [FromQuery] PublicObjectType[] types, [FromQuery] int skip = 0, [FromQuery] int take = 100)
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
    [Route("{buildingId}/objectsByBuilding")]
    public async Task<ActionResult<List<FullBuildingObjectDto>>> GetAllByBuilding(Guid buildingId, [FromQuery] int skip = 0, [FromQuery] int take = 100)
    {
        var getObjects = await buildingObjectsService.GetAllByBuildingId(buildingId, skip, take).ConfigureAwait(false);
        if (getObjects.IsFailure)
        {
            var apiError = getObjects.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        var publicObjects = mapper.Map<List<FullBuildingObjectDto>>(getObjects.Data);
        return Ok(publicObjects);
    }
    
    [HttpGet]
    [Route("{buildingId}/objectsByFloor/{floor}")]
    public async Task<ActionResult<List<FullBuildingObjectDto>>> GetAllByFloor(Guid buildingId, int floor, [FromQuery] int skip = 0, [FromQuery] int take = 100)
    {
        var getObjects = await buildingObjectsService.GetAllByFloor(buildingId, floor, skip, take).ConfigureAwait(false);
        if (getObjects.IsFailure)
        {
            var apiError = getObjects.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        var publicObjects = mapper.Map<List<FullBuildingObjectDto>>(getObjects.Data);
        return Ok(publicObjects);
    }

    [HttpGet]
    [Route("{objectId}")]
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