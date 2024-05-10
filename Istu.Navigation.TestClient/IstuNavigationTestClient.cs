using Istu.Navigation.TestClient.SubsidiaryClients;

namespace Istu.Navigation.TestClient;

public class IstuNavigationTestClient
{
    private IstuNavigationTestClient(IRoutesClient routesClient, IBuildingsClient buildingsClient,
        IEdgesClient edgesClient, IBuildingObjectsClient buildingObjectsClient)
    {
        RoutesClient = routesClient;
        BuildingsClient = buildingsClient;
        EdgesClient = edgesClient;
        BuildingObjectsClient = buildingObjectsClient;
    }

    public IRoutesClient RoutesClient { get; init; }
    public IBuildingsClient BuildingsClient { get; init; }
    
    public IBuildingObjectsClient BuildingObjectsClient { get; init; }
    public IEdgesClient EdgesClient { get; init; }

    public static IstuNavigationTestClient Create(HttpClient? httpClient = null)
    {
        httpClient ??= new HttpClient();
        var routesClient = new RoutesClient(httpClient);
        var buildingsClient = new BuildingsClient(httpClient);
        var edgesClient = new EdgesClient(httpClient);
        var buildingObjectsClient = new BuildingObjectsClient(httpClient);
        return new IstuNavigationTestClient(routesClient, buildingsClient, edgesClient, buildingObjectsClient);
    }
}
