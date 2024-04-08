using Istu.Navigation.TestClient.SubsidiaryClients;

namespace Istu.Navigation.TestClient;

public class IstuNavigationTestClient
{
    private IstuNavigationTestClient(IRoutesClient routesClient, IBuildingsClient buildingsClient,
        IEdgesClient edgesClient)
    {
        RoutesClient = routesClient;
        BuildingsClient = buildingsClient;
        EdgesClient = edgesClient;
    }

    public IRoutesClient RoutesClient { get; init; }
    public IBuildingsClient BuildingsClient { get; init; }
    public IEdgesClient EdgesClient { get; init; }

    public static IstuNavigationTestClient Create(HttpClient? client = null)
    {
        client ??= new HttpClient();
        var routesClient = new RoutesClient(client);
        var buildingsClient = new BuildingsClient(client);
        var edgesClient = new EdgesClient(client);
        return new IstuNavigationTestClient(routesClient, buildingsClient, edgesClient);
    }
}
