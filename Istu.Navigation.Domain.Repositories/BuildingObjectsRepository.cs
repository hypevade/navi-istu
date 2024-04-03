using AutoMapper;
using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Infrastructure.EF;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.Errors.RoutesApiErrors;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Domain.Repositories;

public class BuildingObjectsRepository : IBuildingObjectsRepository
{
    private readonly BuildingsDbContext dbContext;
    private readonly IMapper mapper;

    public BuildingObjectsRepository(BuildingsDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<OperationResult<List<BuildingObject>>> GetAllByTypeInBuilding(Guid buildingId, BuildingObjectType[] types, int skip = 0, int take = 100)
    {
        if (types.Length == 0)
            return OperationResult<List<BuildingObject>>.Failure(BuildingRoutesErrors.EmptyListTypesError());

        var objects = await dbContext.Objects.Where(x => x.BuildingId == buildingId && types.Contains(x.Type))
            .Skip(skip)
            .Take(take)
            .ToListAsync()
            .ConfigureAwait(false);
        if (objects.Count == 0)
            return OperationResult<List<BuildingObject>>.Failure(
                BuildingRoutesErrors.BuildingObjectsNotFoundError(buildingId, types));

        return OperationResult<List<BuildingObject>>.Success(mapper.Map<List<BuildingObject>>(objects));
    }

    public async Task<OperationResult<List<BuildingObject>>> GetAllByBuilding(Guid buildingId, int skip = 0, int take = 100)
    {
        var objects = await dbContext.Objects.Where(x => x.BuildingId == buildingId)
            .Skip(skip)
            .Take(take)
            .ToListAsync()
            .ConfigureAwait(false);
        
        if (objects.Count == 0)
            return OperationResult<List<BuildingObject>>.Failure(
                BuildingRoutesErrors.BuildingObjectsNotFoundError(buildingId));

        return OperationResult<List<BuildingObject>>.Success(mapper.Map<List<BuildingObject>>(objects));
    }

    public async Task<OperationResult<List<BuildingObject>>> GetAllByFloor(Guid buildingId, int floor, int skip = 0, int take = 100)
    {
        var objects = await dbContext.Objects.Where(x => x.BuildingId == buildingId && x.Floor == floor)
            .Skip(skip)
            .Take(take)
            .ToListAsync()
            .ConfigureAwait(false);

        if (objects.Count == 0)
            return OperationResult<List<BuildingObject>>.Failure(
                BuildingRoutesErrors.BuildingObjectsNotFoundError(buildingId, floor));

        return OperationResult<List<BuildingObject>>.Success(mapper.Map<List<BuildingObject>>(objects));
    }

    public async Task<OperationResult<BuildingObject>> GetById(Guid buildingObjectId)
    {
        var objectEntity = await dbContext.Objects.FirstOrDefaultAsync(x => x.Id == buildingObjectId)
            .ConfigureAwait(false);
        if (objectEntity is null)
            return OperationResult<BuildingObject>.Failure(
                BuildingRoutesErrors.BuildingObjectNotFoundError(buildingObjectId));
        return OperationResult<BuildingObject>.Success(mapper.Map<BuildingObject>(objectEntity));
    }

    public Task<OperationResult<BuildingObject>> GetDefaultInputByBuildingId(Guid buildingId)
    {
        throw new NotImplementedException();
    }

    public async Task<OperationResult> Create(BuildingObject buildingObject)
    {
        return await Create([buildingObject]);
    }

    public async Task<OperationResult> Create(List<BuildingObject> buildingObjects)
    {
        var entities = mapper.Map<List<BuildingObjectEntity>>(buildingObjects);
        await dbContext.Objects.AddRangeAsync(entities).ConfigureAwait(false);
        await dbContext.SaveChangesAsync();
        return OperationResult.Success();
    }
}