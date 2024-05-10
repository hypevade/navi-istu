using System.ComponentModel.DataAnnotations;
using Istu.Navigation.Domain.Models.BuildingRoutes;

namespace Istu.Navigation.Public.Models.BuildingObjects;

public class UpdateObjectRequest
{
    [Required]
    public Guid BuildingObjectId { get; set; }

    public List<Guid>? AddedEdges { get; set; }
    public List<Guid>? DeletedEdges { get; set; }
    public BuildingObjectType? UpdatedType { get; set; }
    public int? UpdatedFloorNumber { get; set; }
    public string? UpdatedTitle { get; set; }
    public double? UpdatedX { get; set; }
    public double? UpdatedY { get; set; }
    public string? UpdatedDescription { get; set; }
    
    //TODO: добавить добавление и удаление изображений
    public List<Guid>? AddedImages { get; set; }
    public List<Guid>? DeletedImages { get; set; }
}