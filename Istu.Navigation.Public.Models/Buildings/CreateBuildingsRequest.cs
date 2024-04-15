﻿using System.ComponentModel.DataAnnotations;

namespace Istu.Navigation.Public.Models.Buildings;

public class CreateBuildingRequest
{
    [Required]
    public required string Title { get; set; }
    [Required]
    public required List<CreateFloorDto> Floors { get; set; }
    
    public string? Address { get; set; }
    public string? Description { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}