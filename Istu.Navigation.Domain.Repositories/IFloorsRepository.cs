using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Infrastructure.Errors;

namespace Istu.Navigation.Domain.Repositories;

public interface IFloorsRepository
{
    public Task<OperationResult<Floor>> GetById(Guid floorId);
    public Task<OperationResult<List<Floor>>> GetByBuilding(Guid buildingId, int take = 0, int skip = 0);
}