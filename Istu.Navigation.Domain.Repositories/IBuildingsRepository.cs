using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Infrastructure.Errors;

namespace Istu.Navigation.Domain.Repositories;

public interface IBuildingsRepository
{
    public Task<OperationResult<List<Building>>> GetAll(int take = 100, int skip = 0);
    public Task<OperationResult<List<Building>>> GetAllByTitle(string buildingTitle, int take = 100, int skip = 0);
    public Task<OperationResult<Building>> GetById(Guid buildingId);
}