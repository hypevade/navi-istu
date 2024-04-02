using AutoMapper;
using Istu.Navigation.Domain.Models.BuildingRoutes;
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

    public async Task<OperationResult<List<BuildingObject>>> GetAllByTypeInBuilding(Guid buildingId, BuildingObjectType[] types)
    {
        if (types.Length == 0)
            return OperationResult<List<BuildingObject>>.Failure(BuildingRoutesErrors.EmptyListTypesError());
        
        var objects = await dbContext.Objects.Where(x => x.BuildingId == buildingId && types.Contains(x.Type))
            .ToListAsync()
            .ConfigureAwait(false);
        if (objects.Count == 0)
            return OperationResult<List<BuildingObject>>.Failure(
                BuildingRoutesErrors.BuildingObjectsNotFoundError(buildingId, types));

        return OperationResult<List<BuildingObject>>.Success(mapper.Map<List<BuildingObject>>(objects));
    }

    public async Task<OperationResult<List<BuildingObject>>> GetAllByBuilding(Guid buildingId, int startFloor = default, int endFloor = default)
    {
        var objects = await dbContext.Objects.Where(x => x.BuildingId == buildingId)
            .ToListAsync()
            .ConfigureAwait(false);
        
        if (objects.Count == 0)
            return OperationResult<List<BuildingObject>>.Failure(
                BuildingRoutesErrors.BuildingObjectsNotFoundError(buildingId));

        return OperationResult<List<BuildingObject>>.Success(mapper.Map<List<BuildingObject>>(objects));
    }

    public async Task<OperationResult<List<BuildingObject>>> GetAllByFloor(Guid buildingId, int floor)
    {
        var objects = await dbContext.Objects.Where(x => x.BuildingId == buildingId && x.Floor == floor)
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
}