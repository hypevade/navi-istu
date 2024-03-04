using Istu.Navigation.Domain.Models;
using Istu.Navigation.Domain.Models.InnerObjects;

namespace Istu.Navigation.Domain.Repositories;

public interface IBuildingObjectsRepository
{
    public Task<List<BuildingObject>> GetAllByTypeInBuilding(Guid buildingId, BuildingObjectType[] types);
    public Task<List<BuildingObject>> GetAllByBuilding(Guid buildingId);
    public Task<List<BuildingObject>> GetAllByFloor(Guid buildingId, int floor);
    public Task<BuildingObject?> GetById(Guid buildingObjectId);
}