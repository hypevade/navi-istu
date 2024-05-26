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
    Task<OperationResult<CreateFloorResponse>> AddFloorAsync(Guid buildingId, CreateFloorRequest request);
    Task<OperationResult<FloorDto>> GetFloorAsync(Guid buildingId, int floorNumber);
    Task<OperationResult> DeleteFloorAsync(Guid buildingId, int floorNumber);
}

public class BuildingsClient : BaseClient, IBuildingsClient
{
    public BuildingsClient(HttpClient client) : base(client)
    {
    }

    public async Task<OperationResult<BuildingDto>> GetBuildingByIdAsync(Guid buildingId)
    {
        var route = ApiRoutes.BuildingsRoutes.GetBuildingWithIdRoute(buildingId);
        return await GetAsync<BuildingDto>(route).ConfigureAwait(false);
    }

    public Task<OperationResult> DeleteBuildingAsync(Guid buildingId)
    {
        var route = ApiRoutes.BuildingsRoutes.DeleteBuildingRoute(buildingId);
        return DeleteAsync(route);
    }

    public Task<OperationResult<CreateBuildingResponse>> CreateBuildingAsync(CreateBuildingRequest request)
    {
        var route = ApiRoutes.BuildingsRoutes.CreateBuildingRoute();
        return PostAsync<CreateBuildingRequest, CreateBuildingResponse>(route, request);
    }

    public Task UpdateBuildingAsync(UpdateBuildingRequest request)
    {
        var route = ApiRoutes.BuildingsRoutes.UpdateBuildingRoute();
        return PatchAsync(route, request);
    }

    public Task<OperationResult<List<BuildingDto>>> GetAllByFilterAsync(BuildingFilter filter)
    {
        var route = ApiRoutes.BuildingsRoutes.GetAllBuildingsRoute();
        var queries = new Dictionary<string, string?>();

        if (filter.BuildingId != null)
            queries.Add(nameof(filter.BuildingId), filter.BuildingId.ToString());
        if (filter.Title != null)
            queries.Add(nameof(filter.Title), filter.Title);

        queries.Add(nameof(filter.Skip), filter.Skip.ToString());
        queries.Add(nameof(filter.Take), filter.Take.ToString());
        return GetAsync<List<BuildingDto>>(route, queries);
    }

    public Task<OperationResult<CreateFloorResponse>> AddFloorAsync(Guid buildingId, CreateFloorRequest request)
    {
        var route = ApiRoutes.BuildingsRoutes.CreateFloorRoute(buildingId);
        return PostAsync<CreateFloorRequest, CreateFloorResponse>(route, request);
    }

    public Task<OperationResult<FloorDto>> GetFloorAsync(Guid buildingId, int floorNumber)
    {
        var route = ApiRoutes.BuildingsRoutes.GetFloorRoute(buildingId, floorNumber);
        return GetAsync<FloorDto>(route);
    }

    public Task<OperationResult> DeleteFloorAsync(Guid buildingId, int floorNumber)
    {
        var route = ApiRoutes.BuildingsRoutes.DeleteFloorRoute(buildingId, floorNumber);
        return DeleteAsync(route);
    }
}