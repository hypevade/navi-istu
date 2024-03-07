using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Infrastructure.Errors;

namespace Istu.Navigation.Domain.Services;

public interface IImageService
{
    public Task<OperationResult<List<string>>> GetObjectImageLinks(Guid objectId);
    
    public Task<OperationResult<ImageLink>> GetImageLink(Guid buildingId, int floor);
}