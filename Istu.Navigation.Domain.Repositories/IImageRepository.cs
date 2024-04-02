using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Infrastructure.Errors;

namespace Istu.Navigation.Domain.Repositories;

public interface IImageRepository
{
    public Task<OperationResult<ImageLink>> GetById(Guid imageId); 
    //public Task<OperationResult<List<ImageLink>>> GetAllByObjectId(Guid objectId);
}