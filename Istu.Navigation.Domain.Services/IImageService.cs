namespace Istu.Navigation.Domain.Services;

public interface IImageService
{
    public Task<List<string>> GetObjectImageLinks(Guid objectId);
    
    public Task<string> GetFloorImageLink(Guid buildingId, int floor);
}