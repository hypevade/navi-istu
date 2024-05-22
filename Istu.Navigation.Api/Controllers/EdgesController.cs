using AutoMapper;
using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Api.Filters;
using Istu.Navigation.Api.Paths;
using Istu.Navigation.Domain.Models.Users;
using Istu.Navigation.Domain.Services.Buildings;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Public.Models.BuildingRoutes;
using Istu.Navigation.Public.Models.Edges;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route(ApiRoutes.BuildingEdges.EdgesApi)]
public class EdgesController : ControllerBase
{
    private readonly IEdgesService edgesService;
    private readonly IMapper mapper;

    public EdgesController(IEdgesService edgesService, IMapper mapper)
    {
        this.edgesService = edgesService;
        this.mapper = mapper;
    }
    
    [HttpPost]
    [Route(ApiRoutes.BuildingEdges.CreatePart)]
    [AuthorizationFilter(UserRole.Admin)]
    public async Task<ActionResult<CreateEdgesResponse>> CreateEdges([FromBody] CreateEdgesRequest request)
    {
        var edges = request.Edges.Select(x => (x.FromId, x.ToId)).ToList();
        var createEdgeOperation = await edgesService.CreateRangeAsync(edges).ConfigureAwait(false);

        if (createEdgeOperation.IsFailure)
        {
            var apiError = createEdgeOperation.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        return Ok(new CreateEdgesResponse { EdgeIds = createEdgeOperation.Data });
    }
    
    [HttpGet]
    [Route(ApiRoutes.BuildingEdges.GetAllPart)]
    [AuthorizationFilter(UserRole.User)]
    public async Task<ActionResult<List<EdgeDto>>> GetAllEdges([FromQuery] EdgeFilter filter)
    {
        var getEdgesOperation = await edgesService.GetAllByFilterAsync(filter).ConfigureAwait(false);
        
        if (getEdgesOperation.IsFailure)
        {
            var apiError = getEdgesOperation.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        return Ok(mapper.Map<List<EdgeDto>>(getEdgesOperation.Data));
    }

    [HttpDelete]
    [Route(ApiRoutes.BuildingEdges.DeletePart)]
    [AuthorizationFilter(UserRole.Admin)]
    public async Task<IActionResult> DeleteEdge(Guid edgeId)
    {
        var deleteRangeOperation = await edgesService.DeleteAsync(edgeId).ConfigureAwait(false);

        return deleteRangeOperation.IsSuccess
            ? NoContent()
            : StatusCode(deleteRangeOperation.ApiError.StatusCode, deleteRangeOperation.ApiError.ToErrorDto());
    }
}