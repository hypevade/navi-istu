using System.Web;
using Istu.Navigation.Api.Paths;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Public.Models.BuildingRoutes;

namespace Istu.Navigation.TestClient.SubsidiaryClients;

public interface IEdgesClient
{
    Task<List<EdgeDto>> GetAllEdgesByFilter(EdgeFilter filter);
    //TODO: Create, Delete
}

public class EdgesClient : BaseClient, IEdgesClient
{
    public EdgesClient(HttpClient client) : base(client)
    {
    }

    public async Task<List<EdgeDto>> GetAllEdgesByFilter(EdgeFilter filter)
    {
        var url = ApiRoutes.BuildingEdges.GetAllRoute();
        var queries = new Dictionary<string, string?>();

        if (filter.BuildingId.HasValue)
            queries[nameof(filter.BuildingId)] = filter.BuildingId.Value.ToString();
        if (filter.BuildingObjectId.HasValue)
            queries[nameof(filter.BuildingObjectId)] = filter.BuildingObjectId.Value.ToString();
        if (filter.FloorNumber.HasValue)
            queries[nameof(filter.FloorNumber)] = filter.FloorNumber.Value.ToString();
        if (filter.Skip > 0)
            queries[nameof(filter.Skip)] = filter.Skip.ToString();
        if (filter.Take > 0)
            queries[nameof(filter.Take)] = filter.Take.ToString();
        
        var result = await GetAsync<List<EdgeDto>>(url, queries).ConfigureAwait(false);
        return result;
    }
}
