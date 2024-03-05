using Istu.Navigation.Infrastructure.Errors;

namespace Istu.Navigation.Domain.Services;

public interface IImageService
{
    public Task<OperationResult<List<string>>> GetObjectImageLinks(Guid objectId);
    
    public Task<OperationResult<string>> GetFloorImageLink(Guid buildingId, int floor);
}