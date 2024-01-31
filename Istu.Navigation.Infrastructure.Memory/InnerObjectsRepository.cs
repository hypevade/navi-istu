using Istu.Navigation.Domain.Models;
using Istu.Navigation.Domain.Repositories;

namespace Istu.Navigation.Infrastructure.Memory;

public class InnerObjectsRepository : IInnerObjectsRepository
{
    public Task<InnerObject[]> GetAllByTypeInBuilding(Guid buildingId, InnerObjectType[] types)
    {
        throw new NotImplementedException();
    }

    public Task<InnerObject[]> GetAllByBuilding(Guid buildingId)
    {
        throw new NotImplementedException();
    }

    public Task<InnerObject[]> GetAllByFloor(Guid buildingId, int floor)
    {
        throw new NotImplementedException();
    }

    public Task<InnerObject?> GetById(Guid innerObjectId)
    {
        throw new NotImplementedException();
    }
}