using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Infrastructure.Errors;

namespace Istu.Navigation.Domain.Repositories;

public interface IFloorsRepository
{
    public Task<OperationResult<Floor>> GetById(Guid buildingId, int floorNumber);
    public Task<OperationResult<List<Floor>>> GetAllByBuilding(Guid buildingId);
}