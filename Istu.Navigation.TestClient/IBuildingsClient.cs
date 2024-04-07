using Istu.Navigation.Api.Paths;
using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Public.Models;

namespace Istu.Navigation.TestClient;

public interface IBuildingsClient
{
    Task<BuildingDto> GetBuildingByIdAsync(Guid buildingId);
    Task DeleteBuildingAsync(Guid buildingId);
    Task<CreateBuildingResponse> CreateBuildingAsync(CreateBuildingRequest request);
    Task<Guid> UpdateBuildingAsync(UpdateBuildingsRequest request);
    //Task<List<BuildingDto>> GetAllAsync(UpdateBuildingsRequest request, int skip = 0, int take = 100);
}

public class BuildingsClient : BaseClient, IBuildingsClient
{
    public BuildingsClient(HttpClient client) : base(client)
    {
    }

    public async Task<BuildingDto> GetBuildingByIdAsync(Guid buildingId)
    {
        var route = ApiRoutes.Buildings.GetWithIdRoute(buildingId);
        var response = await GetAsync<BuildingDto>(route).ConfigureAwait(false);
        return response;
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

    public Task<Guid> UpdateBuildingAsync(UpdateBuildingsRequest request)
    {
        throw new NotImplementedException();
    }
}