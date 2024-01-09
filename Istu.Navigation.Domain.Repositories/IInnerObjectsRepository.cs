using Istu.Navigation.Domain.Models;

namespace Istu.Navigation.Domain.Repositories;

public interface IInnerObjectsRepository
{
    public InnerObject[] GetAllByTypeInBuilding(Guid buildingId, InnerObjectType[] types);
    public InnerObject[] GetAllByBuilding(Guid buildingId);
    public InnerObject[] GetAllByFloor(Guid buildingId, int floor);
    public InnerObject GetById(Guid innerObjectId);
}