namespace Istu.Navigation.Domain.Models.Entities;

public class EdgeDto
{
    public required Guid Id { get; set; }
    public required Guid FromObjectId { get; set; }
    public required Guid ToObjectId { get; set; }
    
    public DateTime CreatedAt { get; set; }
}