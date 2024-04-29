using Istu.Navigation.Api.Paths;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Public.Models.Buildings;

namespace Istu.Navigation.TestClient.SubsidiaryClients;

public interface IBuildingsClient
{
    Task<OperationResult<BuildingDto>> GetBuildingByIdAsync(Guid buildingId);
    Task<OperationResult> DeleteBuildingAsync(Guid buildingId);
    Task<OperationResult<CreateBuildingResponse>> CreateBuildingAsync(CreateBuildingRequest request);
    Task UpdateBuildingAsync(UpdateBuildingRequest request);
    Task<OperationResult<List<BuildingDto>>> GetAllByFilterAsync(BuildingFilter filter);
}

public class BuildingsClient : BaseClient, IBuildingsClient
{
    public BuildingsClient(HttpClient client) : base(client)
    {
    }

    public async Task<OperationResult<BuildingDto>> GetBuildingByIdAsync(Guid buildingId)
    {
        var route = ApiRoutes.Buildings.GetWithIdRoute(buildingId);
        return await GetAsync1<BuildingDto>(route).ConfigureAwait(false);
    }

    public Task<OperationResult> DeleteBuildingAsync(Guid buildingId)
    {
        var route = ApiRoutes.Buildings.DeleteRoute(buildingId);
        return DeleteAsync1(route);
    }

    public Task<OperationResult<CreateBuildingResponse>> CreateBuildingAsync(CreateBuildingRequest request)
    {
        var route = ApiRoutes.Buildings.CreateRoute();
        return PostAsync1<CreateBuildingRequest, CreateBuildingResponse>(route, request);
    }

    public Task UpdateBuildingAsync(UpdateBuildingRequest request)
    {
        var route = ApiRoutes.Buildings.UpdateRoute();
        return PatchAsync(route, request);
    }

    public Task<OperationResult<List<BuildingDto>>> GetAllByFilterAsync(BuildingFilter filter)
    {
        var route = ApiRoutes.Buildings.GetAllRoute();
        var queries = new Dictionary<string, string?>();

        if (filter.BuildingId != null)
            queries.Add(nameof(filter.BuildingId), filter.BuildingId.ToString());
        if (filter.Title != null)
            queries.Add(nameof(filter.Title), filter.Title);
        if (filter.FloorNumber != null)
            queries.Add(nameof(filter.FloorNumber), filter.FloorNumber.ToString());

        queries.Add(nameof(filter.Skip), filter.Skip.ToString());
        queries.Add(nameof(filter.Take), filter.Take.ToString());
        return GetAsync1<List<BuildingDto>>(route, queries);
    }
}