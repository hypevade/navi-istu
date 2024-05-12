using AutoMapper;
using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Repositories.Buildings;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.RoutesApiErrors;

namespace Istu.Navigation.Domain.Services.Buildings;

public interface IBuildingObjectsService
{
    public Task<OperationResult<Guid>> Create(BuildingObject buildingObject);
    public Task<OperationResult> Patch(List<BuildingObject> buildingObjects);
    public Task<OperationResult> Delete(List<Guid> buildingObjectsIds);
    public Task<OperationResult<BuildingObject>> GetById(Guid id);
    public Task<OperationResult<List<BuildingObject>>> GetAllByFilter(BuildingObjectFilter filter);
}

public class BuildingObjectsService : IBuildingObjectsService
{
    private readonly IBuildingObjectsRepository buildingObjectsRepository;
    private readonly IBuildingsRepository buildingsRepository;
    private readonly IMapper mapper;

    public BuildingObjectsService(IBuildingObjectsRepository buildingObjectsRepository, IMapper mapper,
        IBuildingsRepository buildingsRepository)
    {
        this.buildingObjectsRepository = buildingObjectsRepository;
        this.mapper = mapper;
        this.buildingsRepository = buildingsRepository;
    }

    public async Task<OperationResult<Guid>> Create(BuildingObject buildingObject)
    {
        var check = await CheckBuildingObject(buildingObject).ConfigureAwait(false);
        if (check.IsFailure)
            return OperationResult<Guid>.Failure(check.ApiError);
        var entity = mapper.Map<BuildingObjectEntity>(buildingObject);
        var result = await buildingObjectsRepository.AddAsync(entity).ConfigureAwait(false);
        await buildingObjectsRepository.SaveChangesAsync().ConfigureAwait(false);
        return OperationResult<Guid>.Success(result.Id);
    }

    public async Task<OperationResult> Patch(List<BuildingObject> buildingObjects)
    {
        foreach (var buildingObject in buildingObjects)
        {
            var check = await CheckBuildingObject(buildingObject, checkExist: false).ConfigureAwait(false);
            if (check.IsFailure)
                return check;
        }

        var buildingObjectsEntity = mapper.Map<List<BuildingObjectEntity>>(buildingObjects);
        buildingObjectsRepository.UpdateRange(buildingObjectsEntity);
        await buildingObjectsRepository.SaveChangesAsync().ConfigureAwait(false);

        return OperationResult.Success();
    }

    public async Task<OperationResult> Delete(List<Guid> buildingObjectsIds)
    {
        await buildingObjectsRepository.RemoveRangeAsync(buildingObjectsIds).ConfigureAwait(false);
        await buildingObjectsRepository.SaveChangesAsync().ConfigureAwait(false);
        return OperationResult.Success();
    }

    public async Task<OperationResult<BuildingObject>> GetById(Guid id)
    {
        var buildingObject = await buildingObjectsRepository.GetByIdAsync(id).ConfigureAwait(false);
        if (buildingObject is null)
            return OperationResult<BuildingObject>.Failure(BuildingObjectsApiErrors.BuildingObjectNotFoundError(id));
        var result = mapper.Map<BuildingObject>(buildingObject);
        return OperationResult<BuildingObject>.Success(result);
    }

    public async Task<OperationResult<List<BuildingObject>>> GetAllByFilter(BuildingObjectFilter filter)
    {
        var buildingObjectEntities = await buildingObjectsRepository.GetAllByFilterAsync(filter).ConfigureAwait(false);
        var result = mapper.Map<List<BuildingObject>>(buildingObjectEntities);
        return OperationResult<List<BuildingObject>>.Success(result);
    }

    private async Task<OperationResult> CheckBuildingObject(BuildingObject buildingObject, bool checkExist = true)
    {
        var checkX = BuildingObject.CoordinateIsValid(buildingObject.X);
        var checkY = BuildingObject.CoordinateIsValid(buildingObject.Y);
        if (!checkY || !checkX)
            return OperationResult.Failure(
                BuildingObjectsApiErrors.InvalidCoordinatesError(buildingObject.X, buildingObject.Y));

        var getBuilding = await buildingsRepository.GetByIdAsync(buildingObject.BuildingId).ConfigureAwait(false);
        if (getBuilding is null)
            return OperationResult.Failure(BuildingsApiErrors.BuildingWithIdNotFoundError(buildingObject.BuildingId));
        
        return OperationResult.Success();
    }
}