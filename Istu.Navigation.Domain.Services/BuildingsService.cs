﻿using AutoMapper;
using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Repositories;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.Errors.RoutesApiErrors;

namespace Istu.Navigation.Domain.Services;

public interface IBuildingsService
{
    public Task<OperationResult> CreateBuildings(List<Building> buildings);
    public Task<OperationResult> CreateBuilding(Building building);
    public Task<OperationResult> PatchBuilding(Building building);
    public Task<OperationResult> PatchBuildings(List<Building> buildings);
    public Task<OperationResult> DeleteBuilding(Guid id);
    public Task<OperationResult<List<Building>>> GetBuildingsByTitle(string title);
    public Task<OperationResult<Building>> GetBuildingById(Guid id);
}

public class BuildingsService : IBuildingsService
{
    private readonly IBuildingsRepository buildingsRepository;
    private readonly IMapper mapper;

    public BuildingsService(IBuildingsRepository buildingsRepository, IMapper mapper)
    {
        this.buildingsRepository = buildingsRepository;
        this.mapper = mapper;
    }

    public async Task<OperationResult> CreateBuildings(List<Building> buildings)
    {
        foreach (var building in buildings)
        {
            var checkResult = await CheckBuilding(building).ConfigureAwait(false);
            if (checkResult.IsFailure)
                return checkResult;
        }

        var createBuildings = mapper.Map<List<BuildingEntity>>(buildings);
        await buildingsRepository.AddRangeAsync(createBuildings).ConfigureAwait(false);
        await buildingsRepository.SaveChangesAsync().ConfigureAwait(false);
        
        return OperationResult.Success();
    }

    public async Task<OperationResult> CreateBuilding(Building building)
    {
        return await CreateBuildings([building]).ConfigureAwait(false);
    }

    public async Task<OperationResult> PatchBuilding(Building building)
    {
        var check = await CheckBuilding(building, checkExist: false).ConfigureAwait(false);
        if (check.IsFailure)
            return check;
        
        var buildingEntity = mapper.Map<BuildingEntity>(building);
        
        buildingsRepository.Update(buildingEntity);
        await buildingsRepository.SaveChangesAsync().ConfigureAwait(false);
        
        return OperationResult.Success();
    }

    public async Task<OperationResult> PatchBuildings(List<Building> buildings)
    {
        foreach (var building in buildings)
        {
            var check = await CheckBuilding(building, checkExist: false).ConfigureAwait(false);
            if (check.IsFailure)
                return check;
        }

        var buildingEntities = mapper.Map<List<BuildingEntity>>(buildings);
        
        buildingsRepository.UpdateRange(buildingEntities);
        await buildingsRepository.SaveChangesAsync().ConfigureAwait(false);
        
        return OperationResult.Success();
    }

    public async Task<OperationResult> DeleteBuilding(Guid id)
    {
        await buildingsRepository.RemoveByIdAsync(id).ConfigureAwait(false);
        await buildingsRepository.SaveChangesAsync().ConfigureAwait(false);
        
        return OperationResult.Success();
    }

    public async Task<OperationResult<List<Building>>> GetBuildingsByTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return OperationResult<List<Building>>.Failure(BuildingsErrors.EmptyTitleError());
     
        var buildings = await buildingsRepository.GetAllByTitle(title).ConfigureAwait(false);
        var result = mapper.Map<List<Building>>(buildings.ToList());
        return OperationResult<List<Building>>.Success(result);
    }

    public async Task<OperationResult<Building>> GetBuildingById(Guid id)
    {
        var building = await buildingsRepository.GetByIdAsync(id).ConfigureAwait(false);
        if (building is null)
            return OperationResult<Building>.Failure(BuildingsErrors.BuildingWithIdNotFoundError(id));
        
        var result = mapper.Map<Building>(building);
        return OperationResult<Building>.Success(result);
    }

    private async Task<OperationResult> CheckBuilding(Building building, bool checkExist = true)
    {
        if (string.IsNullOrWhiteSpace(building.Title))
            return OperationResult.Failure(BuildingsErrors.EmptyTitleError());

        if (building.FloorNumbers < 0)
            return OperationResult.Failure(BuildingsErrors.NegativeFloorNumbersError());
        
        if (!checkExist)
            return OperationResult.Success();

        var isExist = await buildingsRepository.GetByIdAsync(building.Id).ConfigureAwait(false) != null;
        return isExist
            ? OperationResult.Failure(BuildingsErrors.BuildingAlreadyExistsError(building.Id))
            : OperationResult.Success();
    }
}