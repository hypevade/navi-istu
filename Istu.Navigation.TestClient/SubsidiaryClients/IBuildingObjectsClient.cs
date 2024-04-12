using Istu.Navigation.Api.Paths;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Public.Models;
using Istu.Navigation.Public.Models.BuildingObjects;

namespace Istu.Navigation.TestClient.SubsidiaryClients;

public interface IBuildingObjectsClient
{
    Task<BuildingObjectDto> GetBuildingObjectByIdAsync(Guid buildingObjectId);
    Task<List<BuildingObjectDto>> GetAllAsync(BuildingObjectFilter filter);
    Task<CreateBuildingObjectResponse> CreateBuildingObjectAsync(CreateBuildingObjectRequest request);
    Task UpdateBuildingObjectAsync(UpdateObjectRequest request);
    Task DeleteBuildingObjectAsync(Guid buildingObjectId);
}

public class BuildingObjectsClient : BaseClient, IBuildingObjectsClient
{
    public BuildingObjectsClient(HttpClient client) : base(client)
    {
    }

    public Task<BuildingObjectDto> GetBuildingObjectByIdAsync(Guid buildingObjectId)
    {
        throw new NotImplementedException();
    }

    public Task<List<BuildingObjectDto>> GetAllAsync(BuildingObjectFilter filter)
    {
        throw new NotImplementedException();
    }

    public Task<CreateBuildingObjectResponse> CreateBuildingObjectAsync(CreateBuildingObjectRequest request)
    {
        throw new NotImplementedException();
    }

    public Task UpdateBuildingObjectAsync(UpdateObjectRequest request)
    {
        throw new NotImplementedException();
    }

    public Task DeleteBuildingObjectAsync(Guid buildingObjectId)
    {
        var route = ApiRoutes.BuildingObjects.DeleteObjectRoute(buildingObjectId);
        return DeleteAsync(route);
    }
}