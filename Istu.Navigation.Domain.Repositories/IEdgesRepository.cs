using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Infrastructure.Errors;

namespace Istu.Navigation.Domain.Repositories;

public interface IEdgesRepository
{
    public Task<OperationResult<List<Edge>>> GetAllByBuildingId(Guid buildingId, int take = 100, int skip = 0);
    public Task<OperationResult<Edge>> GetById(Guid edgeId);
    public Task<OperationResult<List<Edge>>> GetAllByFloor(Guid buildingId, int floor);
    public Task<OperationResult<List<Edge>>> GetAllByBuildingObject(Guid buildingObjectId);
}