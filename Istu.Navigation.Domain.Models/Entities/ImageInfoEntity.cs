namespace Istu.Navigation.Domain.Models.Entities;

public class ImageInfoEntity : BaseEntity
{
    public Guid? ObjectId { get; set; }
    public required string Title { get; set; }
}