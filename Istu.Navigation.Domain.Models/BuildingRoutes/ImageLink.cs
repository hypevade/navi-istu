using Istu.Navigation.Domain.Models.Entities;

namespace Istu.Navigation.Domain.Models.BuildingRoutes;

public class ImageLink
{
    public ImageLink(Guid id, Guid objectId, string link)
    {
        Id = id;
        ObjectId = objectId;
        
        if (string.IsNullOrWhiteSpace(link))
            throw new ArgumentException($"Параметр {nameof(Link)} не может быть пуст",nameof(Link));
        Link = link;
    }

    public Guid Id { get; init; }

    public Guid ObjectId { get; init; }

    public string Link { get; set; }

    //TODO: Может быть надо добавить тип изображенния

    public static ImageLinkDto ToDto(ImageLink imageLink) => new()
        { Id = imageLink.Id, Link = imageLink.Link, ObjectId = imageLink.ObjectId };
}