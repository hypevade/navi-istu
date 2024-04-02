﻿using Istu.Navigation.Domain.Models.BuildingRoutes;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route("api/buildings")]
public class BuildingObjectsController : ControllerBase
{
    [HttpGet]
    [Route("{buildingId}/objectsByTypes")]
    public IActionResult GetAllByTypeInBuilding(Guid buildingId, [FromQuery] BuildingObjectType[] types)
    {
        throw new InvalidOperationException();
    }

    [HttpGet]
    [Route("{buildingId}/objects")]
    public IActionResult GetAllByBuilding(Guid buildingId)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet]
    [Route("{buildingId}/objectsByFloor/{floor}")]
    public IActionResult GetAllByFloor(Guid buildingId, int floor)
    {
        throw new InvalidOperationException();
    }

    [HttpGet]
    [Route("objects/{objectId}")]
    public IActionResult GetById(Guid objectId)
    {
        return NotFound();
    }
    
    
}