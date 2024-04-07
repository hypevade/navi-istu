using Istu.Navigation.Api.Paths;
using Istu.Navigation.Public.Models.BuildingRoutes;

namespace Istu.Navigation.TestClient.SubsidiaryClients;

public interface IRoutesClient
{
    Task<BuildingRouteResponse?> CreateRoute(BuildingRouteRequest request);
}

public class RoutesClient : BaseClient, IRoutesClient
{
    public RoutesClient(HttpClient client) : base(client)
    {
    }

    public async Task<BuildingRouteResponse?> CreateRoute(BuildingRouteRequest request)
    {
        var url = ApiRoutes.BuildingRoutes.CreateRoute();
        var response = await PostAsync<BuildingRouteRequest, BuildingRouteResponse>(url, request).ConfigureAwait(false);
        return response;
    }
}