namespace Istu.Navigation.Domain.Models.Entities;

public class BuildingEntity : BaseEntity
{
    public required string Title { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
    
    public required string Address { get; set; }
    public string? Description { get; set; }
}