﻿using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Models.ExternalRoutes;
using Istu.Navigation.Domain.Repositories.Buildings;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.Errors;
using Istu.Navigation.Infrastructure.Errors.Errors.RoutesApiErrors;

namespace Istu.Navigation.Domain.Services.Buildings;

public interface IBuildingsService
{
    public Task<OperationResult<Guid>> Create(string title, double latitude, double longitude, string address, string? description = null);
    public Task<OperationResult> Patch(Guid id, string? title = null, double? latitude = null, double? longitude = null, string? address = null, string? description = null);
    public Task<OperationResult> Delete(Guid id);
    public Task<OperationResult<Building>> GetById(Guid id);
    public Task<OperationResult<ExternalPoint>> GetBuildingCoordinates(Guid id);
    public Task<OperationResult<List<Building>>> GetAllByFilter(BuildingFilter filter);
    public Task<OperationResult> CheckExist(Guid buildingId);
}

public class BuildingsService : IBuildingsService
{
    private readonly IBuildingsRepository buildingsRepository;
    private readonly IFloorsService floorsService;

    public BuildingsService(IBuildingsRepository buildingsRepository, IFloorsService floorsService)
    {
        this.buildingsRepository = buildingsRepository;
        this.floorsService = floorsService;
    }

    public async Task<OperationResult<Guid>> Create(string title, double latitude, double longitude, string address, string? description = null)
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
            
        buildingEntity = await buildingsRepository.AddAsync(buildingEntity).ConfigureAwait(false);
        await buildingsRepository.SaveChangesAsync().ConfigureAwait(false);
        return OperationResult<Guid>.Success(buildingEntity.Id);
    }

    public async Task<OperationResult> Patch(Guid id, string? title = null, double? latitude = null, double? longitude = null, string? address = null,string? description = null)
    {
        if (title != null)
        {
            var check = await CheckTitle(title).ConfigureAwait(false);
            if (check.IsFailure)
                return check;
        }
        var buildingEntity = await buildingsRepository.GetByIdAsync(id).ConfigureAwait(false);
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
        
        buildingsRepository.Update(buildingEntity);
        await buildingsRepository.SaveChangesAsync().ConfigureAwait(false);
        
        return OperationResult.Success();
    }

    public async Task<OperationResult> Delete(Guid id)
    {
        await buildingsRepository.RemoveByIdAsync(id).ConfigureAwait(false);
        await buildingsRepository.SaveChangesAsync().ConfigureAwait(false);
        
        return OperationResult.Success();
    }

    public async Task<OperationResult<ExternalPoint>> GetBuildingCoordinates(Guid id)
    {
        var buildingEntity = await buildingsRepository.GetByIdAsync(id).ConfigureAwait(false);
        if (buildingEntity is null)
            return OperationResult<ExternalPoint>.Failure(BuildingsApiErrors.BuildingWithIdNotFoundError(id));
        var resultPoint = new ExternalPoint(buildingEntity.Latitude, buildingEntity.Longitude);
        
        return OperationResult<ExternalPoint>.Success(resultPoint);
    }

    public async Task<OperationResult<List<Building>>> GetAllByFilter(BuildingFilter filter)
    {
        var buildingEntities = await buildingsRepository.GetAllByFilterAsync(filter).ConfigureAwait(false);

        var buildingResults = new List<OperationResult<Building>>();
        foreach (var buildingEntity in buildingEntities)
        {
            var buildingResult = await GetBuildingByEntity(buildingEntity).ConfigureAwait(false);
            if(buildingResult.IsFailure)
                return OperationResult<List<Building>>.Failure(buildingResult.ApiError);
            buildingResults.Add(buildingResult);
        }

        return OperationResult<List<Building>>.Success(buildingResults.Select(r => r.Data).ToList());
    }
    
    public async Task<OperationResult> CheckExist(Guid buildingId)
    {
        var buildingEntity = await buildingsRepository.GetByIdAsync(buildingId).ConfigureAwait(false);
        return buildingEntity is null
            ? OperationResult.Failure(BuildingsApiErrors.BuildingWithIdNotFoundError(buildingId))
            : OperationResult.Success();
    }

    public async Task<OperationResult<Building>> GetById(Guid id)
    {
        var buildingEntity = await buildingsRepository.GetByIdAsync(id).ConfigureAwait(false);
        if (buildingEntity is null)
            return OperationResult<Building>.Failure(BuildingsApiErrors.BuildingWithIdNotFoundError(id));

        return await GetBuildingByEntity(buildingEntity).ConfigureAwait(false);
    }
    
    private async Task<OperationResult> CheckTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return OperationResult.Failure(CommonErrors.EmptyTitleError());
        var existWithTitle = await buildingsRepository.ExistWithTitle(title).ConfigureAwait(false);
        if (existWithTitle)
            return OperationResult.Failure(BuildingsApiErrors.BuildingAlreadyExistsError(title));
        return OperationResult.Success();
    }

    private async Task<OperationResult<Building>> GetBuildingByEntity(BuildingEntity buildingEntity)
    {
        var floors = await floorsService.GetFloorInfosByBuilding(buildingEntity.Id).ConfigureAwait(false);
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