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

[ApiController] [Route(ApiRoutes.BuildingEdgesRoutes.EdgesApi)]
public class EdgesController : ControllerBase
{
    private readonly IEdgesService edgesService;
    private readonly IMapper mapper;

    public EdgesController(IEdgesService edgesService, IMapper mapper)
    {
        this.edgesService = edgesService; this.mapper = mapper;
    }
    
    [HttpPost]
    [Route(ApiRoutes.BuildingEdgesRoutes.CreatePart)] [AuthorizationFilter(UserRole.Admin)]
    public async Task<ActionResult<CreateEdgesResponse>> CreateEdges([FromBody] CreateEdgesRequest request)
    {
        var edges = request.Edges.Select(x => (x.FromId, x.ToId)).ToList();
        var createOperation = await edgesService.CreateRangeAsync(edges).ConfigureAwait(false);

        return createOperation.IsFailure
            ? StatusCode(createOperation.ApiError.StatusCode, createOperation.ApiError.ToErrorDto())
            : Ok(new CreateEdgesResponse { EdgeIds = createOperation.Data });
    }
    
    [HttpGet]
    [Route(ApiRoutes.BuildingEdgesRoutes.GetAllPart)] [AuthorizationFilter(UserRole.User)]
    public async Task<ActionResult<List<EdgeDto>>> GetAllEdges([FromQuery] EdgeFilter filter)
    {
        var getOperation = await edgesService.GetAllByFilterAsync(filter).ConfigureAwait(false);
        
        return getOperation.IsFailure
            ? StatusCode(getOperation.ApiError.StatusCode, getOperation.ApiError.ToErrorDto())
            : Ok(mapper.Map<List<EdgeDto>>(getOperation.Data));
    }

    [HttpDelete]
    [Route(ApiRoutes.BuildingEdgesRoutes.DeletePart)] [AuthorizationFilter(UserRole.Admin)]
    public async Task<IActionResult> DeleteEdge(Guid edgeId)
    {
        var deleteOperation = await edgesService.DeleteAsync(edgeId).ConfigureAwait(false);

        return deleteOperation.IsSuccess
            ? NoContent()
            : StatusCode(deleteOperation.ApiError.StatusCode, deleteOperation.ApiError.ToErrorDto());
    }
}