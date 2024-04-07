﻿using Istu.Navigation.Domain.Models.BuildingRoutes;

namespace Istu.Navigation.Domain.Models.Entities;

public class BuildingObjectEntity : BaseEntity 
{
    public required Guid BuildingId { get; set; }
    public required string Title { get; set; }
    public required int Floor { get; set; }
    public string? Description { get; set; }
    public BuildingObjectType Type { get; set; }
    
    public required double X { get; set; }
    public required double Y { get; set; }
}