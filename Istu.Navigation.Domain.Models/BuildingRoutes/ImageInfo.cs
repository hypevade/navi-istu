using Istu.Navigation.Domain.Models.Entities;

namespace Istu.Navigation.Domain.Models.BuildingRoutes;

public class ImageInfo
{
    public ImageInfo(Guid id, Guid objectId, string title)
    {
        Id = id;
        ObjectId = objectId;
        Title = title;
    }
    public Guid Id { get; init; }
    public Guid ObjectId { get; init; }
    public string Title { get; init; }

    public static ImageInfoEntity ToDto(ImageInfo imageInfo) => new()
    {
        Id = imageInfo.Id,
        ObjectId = imageInfo.ObjectId,
        Title = imageInfo.Title,
    };
}