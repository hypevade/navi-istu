namespace Istu.Navigation.Domain.Models.Entities;

public class ImageLinkEntity : BaseEntity
{
    public required string Link { get; set; }
    public required Guid ObjectId { get; set; }
    
    public required string Title { get; set; }
    public bool CreatedByAdmin { get; set; }
}