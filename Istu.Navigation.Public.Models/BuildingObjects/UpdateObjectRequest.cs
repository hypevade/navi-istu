using System.ComponentModel.DataAnnotations;
using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Public.Models.Buildings;

namespace Istu.Navigation.Public.Models.BuildingObjects;

public class UpdateObjectRequest
{
    [Required]
    public Guid BuildingObjectId { get; set; }
    
    public BuildingObjectType? UpdatedType { get; set; }
    public string? UpdatedTitle { get; set; }
    public BuildingPositionDto? UpdatedPosition { get; set; }
    public string? UpdatedDescription { get; set; }
}