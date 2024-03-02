using Istu.Navigation.Domain.Models;
using Istu.Navigation.Domain.Models.InnerObjects;

namespace Istu.Navigation.Domain.Repositories;

public interface IInnerObjectsRepository
{
    public Task<InnerObject[]> GetAllByTypeInBuilding(Guid buildingId, InnerObjectType[] types);
    public Task<InnerObject[]> GetAllByBuilding(Guid buildingId);
    public Task<InnerObject[]> GetAllByFloor(Guid buildingId, int floor);
    public Task<InnerObject?> GetById(Guid innerObjectId);
}