namespace Istu.Navigation.Domain.Models.Entities;

public class BuildingDto
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required int FloorNumbers { get; set; }
    
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}