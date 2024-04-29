using Istu.Navigation.Api.Paths;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Public.Models;
using Istu.Navigation.Public.Models.BuildingObjects;

namespace Istu.Navigation.TestClient.SubsidiaryClients;

public interface IBuildingObjectsClient
{
    Task<OperationResult<BuildingObjectDto>> GetBuildingObjectByIdAsync(Guid buildingObjectId);
    Task<OperationResult<List<BuildingObjectDto>>> GetAllByFilterAsync(BuildingObjectFilter filter);
    Task<OperationResult<CreateBuildingObjectResponse>> CreateBuildingObjectAsync(CreateBuildingObjectRequest request);
    Task<OperationResult> UpdateBuildingObjectAsync(UpdateObjectRequest request);
    Task<OperationResult> DeleteBuildingObjectAsync(Guid buildingObjectId);
}

public class BuildingObjectsClient : BaseClient, IBuildingObjectsClient
{
    public BuildingObjectsClient(HttpClient client) : base(client)
    {
    }


    public async Task<OperationResult<BuildingObjectDto>> GetBuildingObjectByIdAsync(Guid buildingObjectId)
    {
        var route = ApiRoutes.BuildingObjects.GetWithIdRoute(buildingObjectId);
        return await GetAsync1<BuildingObjectDto>(route).ConfigureAwait(false); 
    }

    public Task<OperationResult<List<BuildingObjectDto>>> GetAllByFilterAsync(BuildingObjectFilter filter)
    {
        var route = ApiRoutes.BuildingObjects.GetAllRoute();
        var queries = new Dictionary<string, string?>();

        if (filter.BuildingId != null)
            queries.Add(nameof(filter.BuildingId), filter.BuildingId.ToString());
        if (filter.Title != null)
            queries.Add(nameof(filter.Title), filter.Title);
        if(filter.BuildingObjectId != null)
            queries.Add(nameof(filter.BuildingObjectId), filter.BuildingObjectId.ToString());
        if (filter.Floor != null)
            queries.Add(nameof(filter.Floor), filter.Floor.ToString());
        if (filter.Types != null)
            queries.Add(nameof(filter.Types), string.Join(',', filter.Types));

        queries.Add(nameof(filter.Skip), filter.Skip.ToString());
        queries.Add(nameof(filter.Take), filter.Take.ToString());
        return GetAsync1<List<BuildingObjectDto>>(route, queries);
    }

    public Task<OperationResult<CreateBuildingObjectResponse>> CreateBuildingObjectAsync(CreateBuildingObjectRequest request)
    {
        var route = ApiRoutes.BuildingObjects.CreateRoute();
        return PostAsync1<CreateBuildingObjectRequest, CreateBuildingObjectResponse>(route, request);
    }

    public Task<OperationResult> UpdateBuildingObjectAsync(UpdateObjectRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult> DeleteBuildingObjectAsync(Guid buildingObjectId)
    {
        var route = ApiRoutes.Buildings.DeleteRoute(buildingObjectId);
        return DeleteAsync1(route);
    }
}