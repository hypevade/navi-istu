using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Infrastructure.Errors;

namespace Istu.Navigation.Domain.Repositories;

public class ImageRepository : IImageRepository
{
    
    
    public Task<OperationResult<ImageLink>> GetById(Guid imageId)
    {
        throw new NotImplementedException();
    }
}