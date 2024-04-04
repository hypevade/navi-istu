using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Infrastructure.EF;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.Errors.RoutesApiErrors;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Domain.Repositories;

public class EdgesRepository : IEdgesRepository
{
    private readonly BuildingsDbContext dbContext;
    private readonly IBuildingObjectsRepository buildingObjects;

    public EdgesRepository(BuildingsDbContext dbContext, IBuildingObjectsRepository buildingObjects)
    {
        this.dbContext = dbContext;
        this.buildingObjects = buildingObjects;
    }

    public async Task<OperationResult<List<Edge>>> GetAllByBuildingId(Guid buildingId, int take = 100, int skip = 0)
    {
        var edges = await dbContext.Edges.Where(x => x.BuildingId == buildingId)
            .Skip(skip)
            .Take(take)
            .ToListAsync()
            .ConfigureAwait(false);
        if (!edges.Any())
            return OperationResult<List<Edge>>.Failure(
                BuildingsErrors.EdgesWithBuildingIdNotFoundError(buildingId));

        var result = new List<Edge>();
        foreach (var edgeEntity in edges)
        {
            var getAge = await GetEdgeByEntity(edgeEntity).ConfigureAwait(false);
            if (getAge.IsFailure)
                return OperationResult<List<Edge>>.Failure(getAge.ApiError);
            result.Add(getAge.Data);
        }

        return OperationResult<List<Edge>>.Success(result);
    }

    public async Task<OperationResult<Edge>> GetById(Guid edgeId)
    {
        var edge = await dbContext.Edges.FirstOrDefaultAsync(x => x.Id == edgeId).ConfigureAwait(false);
        if (edge is null)
            return OperationResult<Edge>.Failure(BuildingsErrors.EdgeWithIdNotFoundError(edgeId));
        var result = await GetEdgeByEntity(edge).ConfigureAwait(false);
        return result.IsFailure
            ? OperationResult<Edge>.Failure(result.ApiError)
            : OperationResult<Edge>.Success(result.Data);
    }

    public async Task<OperationResult<List<Edge>>> GetAllByFloor(Guid buildingId, int floor)
    {

        var edges = await dbContext.Edges
            .Where(x => x.BuildingId == buildingId && x.FloorNumber == floor)
            .ToListAsync().ConfigureAwait(false);

        if (edges.Count == 0)
            return OperationResult<List<Edge>>.Failure(
                BuildingsErrors.EdgesWithBuildingIdAndFloorNotFoundError(buildingId, floor));

        var result = new List<Edge>();
        foreach (var edgeEntity in edges)
        {
            var getAge = await GetEdgeByEntity(edgeEntity).ConfigureAwait(false);
            if (getAge.IsFailure)
                return OperationResult<List<Edge>>.Failure(getAge.ApiError);
            result.Add(getAge.Data);
        }

        return OperationResult<List<Edge>>.Success(result);
    }

    public async Task<OperationResult<List<Edge>>> GetAllByBuildingObject(Guid buildingObjectId)
    {
        var edges = await dbContext.Edges
            .Where(x => x.FromObject == buildingObjectId || x.ToObject == buildingObjectId).ToListAsync()
            .ConfigureAwait(false);

        if (!edges.Any())
            return OperationResult<List<Edge>>.Failure(
                BuildingsErrors.EdgesWithBuildingObjectIdNotFoundError(buildingObjectId));

        var result = new List<Edge>();
        foreach (var edgeEntity in edges)
        {
            var getAge = await GetEdgeByEntity(edgeEntity).ConfigureAwait(false);
            if (getAge.IsFailure)
                return OperationResult<List<Edge>>.Failure(getAge.ApiError);
            result.Add(getAge.Data);
        }

        return OperationResult<List<Edge>>.Success(result);
    }

    private async Task<OperationResult<Edge>> GetEdgeByEntity(EdgeEntity edgeEntity)
    {
        var fromBuildingObject = await buildingObjects.GetById(edgeEntity.FromObject).ConfigureAwait(false);
        if (fromBuildingObject.IsFailure)
            return OperationResult<Edge>.Failure(fromBuildingObject.ApiError);

        var toBuildingObject = await buildingObjects.GetById(edgeEntity.ToObject).ConfigureAwait(false);
        if (toBuildingObject.IsFailure)
            return OperationResult<Edge>.Failure(toBuildingObject.ApiError);

        var edge = new Edge(edgeEntity.Id, fromBuildingObject.Data, toBuildingObject.Data, edgeEntity.FloorNumber);
        return OperationResult<Edge>.Success(edge);
    }
}