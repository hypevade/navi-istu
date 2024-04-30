﻿using Istu.Navigation.Api.Paths;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Public.Models.BuildingRoutes;
using Istu.Navigation.Public.Models.Edges;

namespace Istu.Navigation.TestClient.SubsidiaryClients;

public interface IEdgesClient
{
    Task<OperationResult<List<EdgeDto>>> GetAllEdgesByFilter(EdgeFilter filter);
    Task<OperationResult<CreateEdgesResponse>> CreateEdges(CreateEdgesRequest request);
    
    //TODO: Delete
}

public class EdgesClient : BaseClient, IEdgesClient
{
    public EdgesClient(HttpClient client) : base(client)
    {
    }

    public async Task<OperationResult<List<EdgeDto>>> GetAllEdgesByFilter(EdgeFilter filter)
    {
        var url = ApiRoutes.BuildingEdges.GetAllRoute();
        var queries = new Dictionary<string, string?>();

        if (filter.BuildingId.HasValue)
            queries[nameof(filter.BuildingId)] = filter.BuildingId.Value.ToString();
        
        if (filter.FromBuildingObjectId.HasValue)
            queries[nameof(filter.FromBuildingObjectId)] = filter.FromBuildingObjectId.Value.ToString();
        
        if (filter.ToBuildingObjectId.HasValue)
            queries[nameof(filter.ToBuildingObjectId)] = filter.ToBuildingObjectId.Value.ToString();
        
        if (filter.Floor.HasValue)
            queries[nameof(filter.Floor)] = filter.Floor.Value.ToString();
        
        if (filter.Skip > 0)
            queries[nameof(filter.Skip)] = filter.Skip.ToString();
        
        if (filter.Take > 0)
            queries[nameof(filter.Take)] = filter.Take.ToString();

        var result = await GetAsync<List<EdgeDto>>(url, queries).ConfigureAwait(false);
        return result;
    }

    public Task<OperationResult<CreateEdgesResponse>> CreateEdges(CreateEdgesRequest request)
    {
        var url = ApiRoutes.BuildingEdges.CreateRoute();
        var result = PostAsync<CreateEdgesRequest, CreateEdgesResponse>(url, request);
        return result;
    }
    
    
    /*public Task<OperationResult> DeleteEdges(List<Guid> edgesIds)
    {
        var url = ApiRoutes.BuildingEdges.CreateRoute();
        var queries = new Dictionary<string, string?>();

        foreach (var VARIABLE in edgesIds)
        {
            if (filter.Types != null)
                queries.Add(nameof(filter.Types), string.Join(',', filter.Types));
        }
        var result = DeleteAsync<CreateEdgesRequest>(url);
        return result;
    }*/
}
