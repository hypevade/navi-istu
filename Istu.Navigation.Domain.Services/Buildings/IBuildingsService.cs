using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Models.ExternalRoutes;
using Istu.Navigation.Domain.Repositories.Buildings;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.RoutesApiErrors;

namespace Istu.Navigation.Domain.Services.Buildings;

public interface IBuildingsService
{
    public Task<OperationResult<Guid>> CreateAsync(string title, double latitude, double longitude, string address, string? description = null);
    public Task<OperationResult> PatchAsync(Guid id, string? title = null, double? latitude = null, double? longitude = null, string? address = null, string? description = null);
    public Task<OperationResult> DeleteAsync(Guid id);
    public Task<OperationResult<Building>> GetByIdAsync(Guid id);
    public Task<OperationResult<ExternalPoint>> GetBuildingCoordinatesAsync(Guid id);
    public Task<OperationResult<List<Building>>> GetAllByFilterAsync(BuildingFilter filter);
    public Task<OperationResult> CheckExistAsync(Guid buildingId);
}

public class BuildingsService(IBuildingsRepository repository, IFloorsService service)
    : IBuildingsService
{
    public async Task<OperationResult<Guid>> CreateAsync(string title, double latitude, double longitude,
        string address, string? description = null)
    {
        var checkResult = await CheckTitle(title).ConfigureAwait(false);
        if (checkResult.IsFailure)
            return OperationResult<Guid>.Failure(checkResult.ApiError);

        var buildingEntity = new BuildingEntity()
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            Latitude = latitude,
            Longitude = longitude,
            Address = address
        };

        buildingEntity = await repository.AddAsync(buildingEntity).ConfigureAwait(false);
        await repository.SaveChangesAsync().ConfigureAwait(false);
        return OperationResult<Guid>.Success(buildingEntity.Id);
    }

    public async Task<OperationResult> PatchAsync(Guid id, string? title = null, double? latitude = null,
        double? longitude = null, string? address = null, string? description = null)
    {
        if (title != null)
        {
            var check = await CheckTitle(title).ConfigureAwait(false);
            if (check.IsFailure)
                return check;
        }

        var buildingEntity = await repository.GetByIdAsync(id).ConfigureAwait(false);
        if (buildingEntity is null)
            return OperationResult.Failure(BuildingsApiErrors.BuildingWithIdNotFoundError(id));

        if (title != null)
            buildingEntity.Title = title;
        if (latitude.HasValue)
            buildingEntity.Latitude = latitude.Value;
        if (longitude.HasValue)
            buildingEntity.Longitude = longitude.Value;
        if (description != null)
            buildingEntity.Description = description;
        if (address != null)
            buildingEntity.Address = address;

        repository.Update(buildingEntity);
        await repository.SaveChangesAsync().ConfigureAwait(false);

        return OperationResult.Success();
    }

    public async Task<OperationResult> DeleteAsync(Guid id)
    {
        await repository.RemoveByIdAsync(id).ConfigureAwait(false);
        await repository.SaveChangesAsync().ConfigureAwait(false);

        return OperationResult.Success();
    }

    public async Task<OperationResult<ExternalPoint>> GetBuildingCoordinatesAsync(Guid id)
    {
        var buildingEntity = await repository.GetByIdAsync(id).ConfigureAwait(false);
        if (buildingEntity is null)
            return OperationResult<ExternalPoint>.Failure(BuildingsApiErrors.BuildingWithIdNotFoundError(id));
        var resultPoint = new ExternalPoint(buildingEntity.Latitude, buildingEntity.Longitude);

        return OperationResult<ExternalPoint>.Success(resultPoint);
    }

    public async Task<OperationResult<List<Building>>> GetAllByFilterAsync(BuildingFilter filter)
    {
        var buildingEntities = await repository.GetAllByFilterAsync(filter).ConfigureAwait(false);

        var buildingResults = new List<OperationResult<Building>>();
        foreach (var buildingEntity in buildingEntities)
        {
            var buildingResult = await BuildBuildingAsync(buildingEntity).ConfigureAwait(false);
            if (buildingResult.IsFailure)
                return OperationResult<List<Building>>.Failure(buildingResult.ApiError);
            buildingResults.Add(buildingResult);
        }

        return OperationResult<List<Building>>.Success(buildingResults.Select(r => r.Data).ToList());
    }

    public async Task<OperationResult> CheckExistAsync(Guid buildingId)
    {
        var buildingEntity = await repository.GetByIdAsync(buildingId).ConfigureAwait(false);
        return buildingEntity is null
            ? OperationResult.Failure(BuildingsApiErrors.BuildingWithIdNotFoundError(buildingId))
            : OperationResult.Success();
    }

    public async Task<OperationResult<Building>> GetByIdAsync(Guid id)
    {
        var buildingEntity = await repository.GetByIdAsync(id).ConfigureAwait(false);
        if (buildingEntity is null)
            return OperationResult<Building>.Failure(BuildingsApiErrors.BuildingWithIdNotFoundError(id));

        return await BuildBuildingAsync(buildingEntity).ConfigureAwait(false);
    }

    private async Task<OperationResult> CheckTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return OperationResult.Failure(CommonErrors.EmptyTitleError());
        var existWithTitle = await repository.ExistWithTitle(title).ConfigureAwait(false);
        if (existWithTitle)
            return OperationResult.Failure(BuildingsApiErrors.BuildingAlreadyExistsError(title));
        return OperationResult.Success();
    }

    private async Task<OperationResult<Building>> BuildBuildingAsync(BuildingEntity buildingEntity)
    {
        var floors = await service.GetFloorInfosByBuilding(buildingEntity.Id).ConfigureAwait(false);
        if (floors.IsFailure)
        {
            return OperationResult<Building>.Failure(CommonErrors.InternalServerError());
        }

        var building = new Building
        {
            Id = buildingEntity.Id,
            Title = buildingEntity.Title,
            Floors = floors.Data,
            Description = buildingEntity.Description,
            Latitude = buildingEntity.Latitude,
            Longitude = buildingEntity.Longitude,
            Address = buildingEntity.Address
        };
        return OperationResult<Building>.Success(building);
    }
}