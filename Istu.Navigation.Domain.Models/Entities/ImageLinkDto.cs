namespace Istu.Navigation.Domain.Models.Entities;

public class ImageLinkDto
{
    public required Guid Id { get; set; }
    public required Guid ObjectId { get; set; }
    public required string Link { get; set; }
}