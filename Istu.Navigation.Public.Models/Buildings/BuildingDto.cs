using System.ComponentModel.DataAnnotations;

namespace Istu.Navigation.Public.Models.Buildings;

public class BuildingDto
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public required string Title { get; set; }
    [Required]
    public int FloorNumbers { get; set; }

    public string? Description { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}