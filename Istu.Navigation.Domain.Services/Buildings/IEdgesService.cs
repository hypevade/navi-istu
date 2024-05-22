using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Repositories.Buildings;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.RoutesApiErrors;

namespace Istu.Navigation.Domain.Services.Buildings;

public interface IEdgesService
{
    public Task<OperationResult<List<Edge>>> GetAllByFilterAsync(EdgeFilter filter);
    public Task<OperationResult<Edge>> GetByIdAsync(Guid edgeId);
    public Task<OperationResult<Guid>> Create(Guid fromId, Guid toId);
    public Task<OperationResult<List<Guid>>> CreateRangeAsync(List<(Guid fromId, Guid toId)> edges);
    public Task<OperationResult> DeleteAsync(Guid fromId, Guid toId);
    public Task<OperationResult> DeleteAsync(Guid edgeId);
}

public class EdgesService(IEdgesRepository repository, IBuildingObjectsService objectsService) : IEdgesService
{
    public async Task<OperationResult<List<Edge>>> GetAllByFilterAsync(EdgeFilter filter)
    {
        var edgeEntities = await repository.GetAllByFilterAsync(filter).ConfigureAwait(false);
        var edges = new List<Edge>();
        foreach (var edgeEntity in edgeEntities)
        {
            var getEdge = await GetByIdAsync(edgeEntity.Id).ConfigureAwait(false);
            if (getEdge.IsSuccess)
                edges.Add(getEdge.Data);
        }

        return OperationResult<List<Edge>>.Success(edges);
    }

    public async Task<OperationResult<Edge>> GetByIdAsync(Guid edgeId)
    {
        var edge = await repository.GetByIdAsync(edgeId).ConfigureAwait(false);
        if (edge is null)
            return OperationResult<Edge>.Failure(BuildingsErrors.EdgeWithIdNotFoundError(edgeId));
        var getFromObj = await objectsService.GetByIdAsync(edge.FromObject).ConfigureAwait(false);
        if (getFromObj.IsFailure)
            return OperationResult<Edge>.Failure(getFromObj.ApiError);
        var getToObj = await objectsService.GetByIdAsync(edge.ToObject).ConfigureAwait(false);
        if (getToObj.IsFailure)
            return OperationResult<Edge>.Failure(getToObj.ApiError);

        return OperationResult<Edge>.Success(new Edge(edgeId, getFromObj.Data, getToObj.Data, edge.FloorNumber));
    }

    public async Task<OperationResult<Guid>> Create(Guid fromId, Guid toId)
    {
        var result = await CreateRangeAsync([(fromId, toId)]).ConfigureAwait(false);
        return result.IsSuccess
            ? OperationResult<Guid>.Success(result.Data.First())
            : OperationResult<Guid>.Failure(result.ApiError);
    }

    public async Task<OperationResult<List<Guid>>> CreateRangeAsync(List<(Guid fromId, Guid toId)> edges)
    {
        var edgeEntities = new List<EdgeEntity>();
        var existingEdges = new List<Guid>();
        foreach (var edge in edges)
        {
            var getEdgeEntity = await CheckEdgeAndGetEntity(edge.fromId, edge.toId).ConfigureAwait(false);

            var existEdges = await repository
                .GetAllByFilterAsync(new EdgeFilter
                    { FromBuildingObjectId = edge.fromId, ToBuildingObjectId = edge.toId }).ConfigureAwait(false);
            var existEdge = existEdges.FirstOrDefault();
            if (existEdge is not null)
            {
                existingEdges.Add(existEdge.Id);
                continue;
            }

            if (getEdgeEntity.IsFailure)
            {
                return OperationResult<List<Guid>>.Failure(getEdgeEntity.ApiError);
            }

            edgeEntities.Add(getEdgeEntity.Data);
        }

        var addedEdges = await repository.AddRangeAsync(edgeEntities).ConfigureAwait(false);
        await repository.SaveChangesAsync().ConfigureAwait(false);

        return OperationResult<List<Guid>>.Success(addedEdges.Select(x => x.Id).Concat(existingEdges).ToList());
    }

    public async Task<OperationResult> DeleteAsync(Guid fromId, Guid toId)
    {
        var edges = await repository.FindAsync(x =>
                x.FromObject == fromId && x.ToObject == toId || x.FromObject == toId && x.ToObject == fromId)
            .ConfigureAwait(false);

        if (edges.Count == 0)
            return OperationResult.Success();

        await repository.RemoveRangeAsync(edges.Select(x => x.Id)).ConfigureAwait(false);
        await repository.SaveChangesAsync().ConfigureAwait(false);
        return OperationResult.Success();
    }

    public Task<OperationResult> DeleteAsync(Guid edgeId)
    {
        return DeleteRange([edgeId]);
    }

    public async Task<OperationResult> DeleteRange(List<Guid> edgeIds)
    {
        await repository.RemoveRangeAsync(edgeIds).ConfigureAwait(false);
        await repository.SaveChangesAsync().ConfigureAwait(false);
        return OperationResult.Success();
    }

    private async Task<OperationResult<EdgeEntity>> CheckEdgeAndGetEntity(Guid fromId, Guid toId)
    {
        if (fromId == toId)
            return OperationResult<EdgeEntity>.Failure(EdgesApiErrors.EdgeFromToSameError(fromId, toId));


        var fromObj = await objectsService.GetByIdAsync(fromId).ConfigureAwait(false);
        if (fromObj.IsFailure)
            return OperationResult<EdgeEntity>.Failure(fromObj.ApiError);

        var toObj = await objectsService.GetByIdAsync(toId).ConfigureAwait(false);
        if (toObj.IsFailure)
            return OperationResult<EdgeEntity>.Failure(toObj.ApiError);

        if (fromObj.Data.BuildingId != toObj.Data.BuildingId)
            return OperationResult<EdgeEntity>.Failure(EdgesApiErrors.EdgeFromDifferentBuildingsError(fromId, toId));

        return OperationResult<EdgeEntity>.Success(new EdgeEntity()
        {
            Id = Guid.NewGuid(),
            FromObject = fromId,
            ToObject = toId,
            FloorNumber = fromObj.Data.Floor,
            BuildingId = fromObj.Data.BuildingId,
        });
    }
}