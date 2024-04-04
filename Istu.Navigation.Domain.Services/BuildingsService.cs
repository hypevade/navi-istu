using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Repositories;
using Istu.Navigation.Infrastructure.Errors;

namespace Istu.Navigation.Domain.Services;

public interface IBuildingsService
{
    public Task<OperationResult> CreateBuildings(List<Building> buildings);
}

public class BuildingsService : IBuildingsService
{
    private readonly IBuildingsRepository buildingsRepository;

    public BuildingsService(IBuildingsRepository buildingsRepository)
    {
        this.buildingsRepository = buildingsRepository;
    }

    public async Task<OperationResult> CreateBuildings(List<Building> buildings)
    {
        return await buildingsRepository.CreateBuildings(buildings).ConfigureAwait(false);
    }
}