﻿namespace Istu.Navigation.Domain.Models.Entities;

public class BuildingEntity : BaseEntity
{
    public required string Title { get; set; }
    public required int FloorNumbers { get; set; }
    
    public string? Description { get; set; }
}