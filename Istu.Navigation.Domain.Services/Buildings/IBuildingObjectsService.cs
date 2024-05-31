using AutoMapper;
using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Models.Cards;
using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Repositories.Buildings;
using Istu.Navigation.Domain.Services.ExternalRoutes;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.RoutesApiErrors;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace Istu.Navigation.Domain.Services.Buildings;

public interface IBuildingObjectsService
{
    public Task<OperationResult<Guid>> CreateAsync(BuildingObject buildingObject);
    public Task<OperationResult> PatchAsync(Guid buildingObjectId, string? title, string? description, BuildingObjectType? type, double? x, double? y);
    public Task<OperationResult> DeleteAsync(List<Guid> buildingObjectsIds);
    public Task<OperationResult<BuildingObject>> GetByIdAsync(Guid id);
    public Task<OperationResult<List<BuildingObject>>> GetAllByFilterAsync(BuildingObjectFilter filter);
}

public class BuildingObjectsService : IBuildingObjectsService
{
    private readonly IBuildingObjectsRepository buildingObjectsRepository;
    private readonly IBuildingsRepository buildingsRepository;
    private readonly IMapper mapper;
    private readonly ILuceneService luceneService;

    public BuildingObjectsService(IBuildingObjectsRepository buildingObjectsRepository, IMapper mapper,
        IBuildingsRepository buildingsRepository, ILuceneService luceneService)
    {
        this.buildingObjectsRepository = buildingObjectsRepository;
        this.mapper = mapper;
        this.buildingsRepository = buildingsRepository;
        this.luceneService = luceneService;
    }

    public async Task<OperationResult<Guid>> CreateAsync(BuildingObject buildingObject)
    {
        var check = await CheckBuildingObject(buildingObject).ConfigureAwait(false);
        if (check.IsFailure)
            return OperationResult<Guid>.Failure(check.ApiError);
        
        var entity = mapper.Map<BuildingObjectEntity>(buildingObject);
        var result = await buildingObjectsRepository.AddAsync(entity).ConfigureAwait(false);
        await buildingObjectsRepository.SaveChangesAsync().ConfigureAwait(false);
        if (entity.Type.IsPublicObject())
            luceneService.AddDocument(result.Id, ContentType.Object, entity.Title ?? entity.Type.GetRussianName(),
                buildingObject.Keywords ?? "", buildingObject.Description ?? "");
        
        return OperationResult<Guid>.Success(result.Id);
    }

    public Task<OperationResult> PatchAsync(Guid buildingObjectId, string? title, string? description, BuildingObjectType? type, double? x,
        double? y)
    {
        var buildingObject = buildingObjectsRepository.GetByIdAsync(buildingObjectId).Result;
        if (buildingObject is null)
            return Task.FromResult(OperationResult.Failure(BuildingObjectsApiErrors.BuildingObjectNotFoundError(buildingObjectId)));

        if (title != null) buildingObject.Title = title;
        if (description != null) buildingObject.Description = description;
        if (type.HasValue) buildingObject.Type = type.Value;
        if (x.HasValue) buildingObject.X = x.Value;
        if (y.HasValue) buildingObject.Y = y.Value;
        
        buildingObjectsRepository.Update(buildingObject);
        buildingObjectsRepository.SaveChangesAsync();
        
        return Task.FromResult(OperationResult.Success());
    }

    public async Task<OperationResult> DeleteAsync(List<Guid> buildingObjectsIds)
    {
        await buildingObjectsRepository.RemoveRangeAsync(buildingObjectsIds).ConfigureAwait(false);
        await buildingObjectsRepository.SaveChangesAsync().ConfigureAwait(false);

        buildingObjectsIds.ForEach(id => luceneService.DeleteDocument(id));
        
        return OperationResult.Success();
    }

    public async Task<OperationResult<BuildingObject>> GetByIdAsync(Guid id)
    {
        var buildingObject = await buildingObjectsRepository.GetByIdAsync(id).ConfigureAwait(false);
        if (buildingObject is null)
            return OperationResult<BuildingObject>.Failure(BuildingObjectsApiErrors.BuildingObjectNotFoundError(id));
        
        var result = mapper.Map<BuildingObject>(buildingObject);
        return OperationResult<BuildingObject>.Success(result);
    }

    public async Task<OperationResult<List<BuildingObject>>> GetAllByFilterAsync(BuildingObjectFilter filter)
    {
        var buildingObjectEntities = await buildingObjectsRepository.GetAllByFilterAsync(filter).ConfigureAwait(false);
        var result = mapper.Map<List<BuildingObject>>(buildingObjectEntities);
        return OperationResult<List<BuildingObject>>.Success(result);
    }

    private async Task<OperationResult> CheckBuildingObject(BuildingObject buildingObject)
    {
        var checkX = BuildingObject.CoordinateIsValid(buildingObject.X);
        var checkY = BuildingObject.CoordinateIsValid(buildingObject.Y);
        if (!checkY || !checkX)
            return
                OperationResult.Failure(BuildingObjectsApiErrors.InvalidCoordinatesError(buildingObject.X, buildingObject.Y));

        var getBuilding = await buildingsRepository.GetByIdAsync(buildingObject.BuildingId).ConfigureAwait(false);
        if (getBuilding is null)
            return OperationResult.Failure(BuildingsApiErrors.BuildingWithIdNotFoundError(buildingObject.BuildingId));
        
        return OperationResult.Success();
    }
}