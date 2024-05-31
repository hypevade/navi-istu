﻿namespace Istu.Navigation.Public.Models.BuildingRoutes;

public class BuildingRouteRequest
{
    public required Guid ToId { get; set; }
    public Guid? FromId { get; set; }
    public bool DisableElevator { get; set; } = false;
}