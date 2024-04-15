using System.ComponentModel.DataAnnotations;

namespace Istu.Navigation.Public.Models.Buildings;

public class BuildingDto
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public required string Title { get; set; }

    public required List<FloorInfoDto> Floors { get; set; }

    public string? Description { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}