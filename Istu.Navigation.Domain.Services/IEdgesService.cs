using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Repositories;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.Errors.RoutesApiErrors;

namespace Istu.Navigation.Domain.Services;

public interface IEdgesService
{
    public Task<OperationResult<List<Edge>>> GetAllByFloor(Guid buildingId, int floorNumber);
    public Task<OperationResult<Edge>> GetById(Guid edgeId);
    public Task<OperationResult> Create(Guid fromId, Guid toId);
    public Task<OperationResult> Delete(Guid fromId, Guid toId);
    public Task<OperationResult> DeleteRange(List<Guid> edgeIds);
}

public class EdgesService : IEdgesService
{
    private readonly IEdgesRepository edgesRepository;
    private readonly IBuildingObjectsService buildingObjectsService;

    public EdgesService(IEdgesRepository edgesRepository, IBuildingObjectsService buildingObjectsService)
    {
        this.edgesRepository = edgesRepository;
        this.buildingObjectsService = buildingObjectsService;
    }

    public async Task<OperationResult<List<Edge>>> GetAllByFloor(Guid buildingId, int floorNumber)
    {
        var edgesEntities = await edgesRepository.GetAllByFloor(buildingId, floorNumber).ConfigureAwait(false);
        var edges = new List<Edge>();
        foreach (var edgeEntity in edgesEntities)
        {
            var getEdge = await GetById(edgeEntity.Id).ConfigureAwait(false);
            if (getEdge.IsSuccess)
                edges.Add(getEdge.Data);
        }

        return OperationResult<List<Edge>>.Success(edges);
    }

    //Подумать над тем как можно оптимизировать. 
    public async Task<OperationResult<Edge>> GetById(Guid edgeId)
    {
        var edge = await edgesRepository.GetByIdAsync(edgeId).ConfigureAwait(false);
        if (edge is null)
            return OperationResult<Edge>.Failure(BuildingsErrors.EdgeWithIdNotFoundError(edgeId));
        var getFromObj = await buildingObjectsService.GetById(edge.FromObject).ConfigureAwait(false);
        if (getFromObj.IsFailure)
            return OperationResult<Edge>.Failure(getFromObj.ApiError);
        var getToObj = await buildingObjectsService.GetById(edge.ToObject).ConfigureAwait(false);
        if (getToObj.IsFailure)
            return OperationResult<Edge>.Failure(getToObj.ApiError);

        return OperationResult<Edge>.Success(new Edge(edgeId, getFromObj.Data, getToObj.Data, edge.FloorNumber));
    }

    public async Task<OperationResult> Create(Guid fromId, Guid toId)
    {
        var fromObj = await buildingObjectsService.GetById(fromId).ConfigureAwait(false);
        if (fromObj.IsFailure)
            return OperationResult.Failure(fromObj.ApiError);

        var toObj = await buildingObjectsService.GetById(toId).ConfigureAwait(false);
        if (toObj.IsFailure)
            return OperationResult.Failure(toObj.ApiError);

        var edgeEntity = new EdgeEntity()
        {
            Id = Guid.NewGuid(),
            FromObject = fromId,
            ToObject = toId,
            FloorNumber = fromObj.Data.FloorNumber,
            BuildingId = fromObj.Data.BuildingId,
        };

        await edgesRepository.AddAsync(edgeEntity).ConfigureAwait(false);
        await edgesRepository.SaveChangesAsync().ConfigureAwait(false);
        return OperationResult.Success();
    }


    public async Task<OperationResult> Delete(Guid fromId, Guid toId)
    {
        var edges = await edgesRepository.FindAsync(x =>
                x.FromObject == fromId && x.ToObject == toId || x.FromObject == toId && x.ToObject == fromId)
            .ConfigureAwait(false);

        edgesRepository.RemoveRange(edges);
        return OperationResult.Success();
    }

    public async Task<OperationResult> DeleteRange(List<Guid> edgeIds)
    {
        await edgesRepository.RemoveRangeAsync(edgeIds).ConfigureAwait(false);
        await edgesRepository.SaveChangesAsync().ConfigureAwait(false);
        return OperationResult.Success();
    }
}