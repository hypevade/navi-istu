using AutoMapper;
using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Services;
using Istu.Navigation.Public.Models;
using Istu.Navigation.Public.Models.BuildingRoutes;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route("api/buildings")]
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
    [Route("objects")]
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
    [Route("{buildingId}/objects")]
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
    [Route("{buildingId}/objectsByFloor")]
    public async Task<ActionResult<List<FullBuildingObjectDto>>> GetAllByFloor(Guid buildingId,[FromQuery] int floor, [FromQuery] int skip = 0, [FromQuery] int take = 100)
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
    [Route("objects/{objectId}")]
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
    
    [HttpGet]
    [Route("objects/{objectId}/edges")]
    public async Task<ActionResult<List<EdgeDto>>> GetEdgesById(Guid objectId)
    {
        var getEdgesOperation = await edgesService.GetAllByObject(objectId).ConfigureAwait(false);
        if (getEdgesOperation.IsFailure) 
        {
            var apiError = getEdgesOperation.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        return Ok(mapper.Map<List<EdgeDto>>(getEdgesOperation.Data));
    }

    [HttpPost]
    [Route("objects/edges")]
    public async Task<IActionResult> CreateEdges([FromBody] CreateEdgesRequest request)
    {
        var edges = request.Edges.Select(x => (x.FromId, x.ToId)).ToList();
        var createEdgeOperation = await edgesService.CreateRange(edges).ConfigureAwait(false);

        if (createEdgeOperation.IsFailure)
        {
            var apiError = createEdgeOperation.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        return Ok(new CreateEdgesResponse { EdgeIds = createEdgeOperation.Data });
    }
}