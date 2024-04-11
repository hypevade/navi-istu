﻿using AutoMapper;
using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Api.Paths;
using Istu.Navigation.Domain.Services;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Public.Models;
using Istu.Navigation.Public.Models.BuildingRoutes;
using Istu.Navigation.Public.Models.Edges;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route(ApiRoutes.BuildingEdges.BuildingsRoutesApi)]
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
    
    [HttpGet]
    [Route(ApiRoutes.BuildingEdges.GetAllPart)]
    public async Task<ActionResult<List<EdgeDto>>> GetAllEdges([FromQuery] EdgeFilter filter)
    {
        var r = HttpContext.Request;
        var getEdgesOperation = await edgesService.GetAllByFilter(filter).ConfigureAwait(false);
        
        if (getEdgesOperation.IsFailure)
        {
            var apiError = getEdgesOperation.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        return Ok(mapper.Map<List<EdgeDto>>(getEdgesOperation.Data));
    }
}