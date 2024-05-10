using Istu.Navigation.Api.Paths;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Public.Models.BuildingRoutes;

namespace Istu.Navigation.TestClient.SubsidiaryClients;

public interface IRoutesClient
{
    Task<OperationResult<BuildingRouteResponse>> CreateRoute(BuildingRouteRequest request);
}

public class RoutesClient : BaseClient, IRoutesClient
{
    public RoutesClient(HttpClient client) : base(client)
    {
    }

    public Task<OperationResult<BuildingRouteResponse>> CreateRoute(BuildingRouteRequest request)
    {
        var url = ApiRoutes.BuildingRoutes.CreateRoute();
        return PostAsync<BuildingRouteRequest, BuildingRouteResponse>(url, request);
    }
}