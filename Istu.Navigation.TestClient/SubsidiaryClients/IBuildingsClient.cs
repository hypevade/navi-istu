using Istu.Navigation.Api.Paths;
using Istu.Navigation.Public.Models.Buildings;

namespace Istu.Navigation.TestClient.SubsidiaryClients;

public interface IBuildingsClient
{
    Task<BuildingDto> GetBuildingByIdAsync(Guid buildingId);
    Task DeleteBuildingAsync(Guid buildingId);
    Task<CreateBuildingResponse> CreateBuildingAsync(CreateBuildingRequest request);
    Task UpdateBuildingAsync(UpdateBuildingRequest request);
    Task<List<BuildingDto>> GetAllAsync(int skip = 0, int take = 100);
}

public class BuildingsClient : BaseClient, IBuildingsClient
{
    public BuildingsClient(HttpClient client) : base(client)
    {
    }

    public async Task<BuildingDto> GetBuildingByIdAsync(Guid buildingId)
    {
        var route = ApiRoutes.Buildings.GetWithIdRoute(buildingId);
        return await GetAsync<BuildingDto>(route).ConfigureAwait(false);
    }

    public Task DeleteBuildingAsync(Guid buildingId)
    {
        var route = ApiRoutes.Buildings.DeleteRoute(buildingId);
        return DeleteAsync(route);
    }

    public Task<CreateBuildingResponse> CreateBuildingAsync(CreateBuildingRequest request)
    {
        var route = ApiRoutes.Buildings.CreateRoute();
        return PostAsync<CreateBuildingRequest, CreateBuildingResponse>(route, request);
    }

    public Task UpdateBuildingAsync(UpdateBuildingRequest request)
    {
        var route = ApiRoutes.Buildings.UpdateRoute();
        return PatchAsync(route, request);
    }

    public Task<List<BuildingDto>> GetAllAsync(int skip = 0, int take = 100)
    {
        var route = ApiRoutes.Buildings.GetAllRoute();
        return GetAsync<List<BuildingDto>>(route);
    }
}