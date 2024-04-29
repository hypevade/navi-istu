using Istu.Navigation.Domain.Models.Entities;

namespace Istu.Navigation.Domain.Models.BuildingRoutes;

public class ImageLink
{
    public ImageLink(Guid id, Guid objectId, string link, string title)
    {
        Id = id;
        ObjectId = objectId;
        Link = link;
        Title = title;
    }

    public Guid Id { get; init; }
    public Guid ObjectId { get; init; }
    public string Title { get; init; }

    public string Link { get; set; }

    //TODO: Может быть надо добавить тип изображенния

    public static ImageLinkEntity ToDto(ImageLink imageLink) => new()
    {
        Id = imageLink.Id,
        Link = imageLink.Link,
        ObjectId = imageLink.ObjectId,
        Title = imageLink.Title,
    };
}