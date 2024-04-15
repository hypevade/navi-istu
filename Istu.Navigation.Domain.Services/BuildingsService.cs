using AutoMapper;
using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Repositories;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.Errors;
using Istu.Navigation.Infrastructure.Errors.Errors.RoutesApiErrors;

namespace Istu.Navigation.Domain.Services;

public interface IBuildingsService
{
    public Task<OperationResult<Guid>> Create(Building building);
    public Task<OperationResult> Patch(Building building);
    public Task<OperationResult> PatchRange(List<Building> buildings);
    public Task<OperationResult> Delete(Guid id);
    public Task<OperationResult<Building>> GetById(Guid id);
    public Task<OperationResult<List<Building>>> GetAllByFilter(BuildingFilter filter);
}

public class BuildingsService : IBuildingsService
{
    private readonly IBuildingsRepository buildingsRepository;
    private readonly IImageService imageService;
    private readonly IMapper mapper;

    public BuildingsService(IBuildingsRepository buildingsRepository, IMapper mapper, IImageService imageService)
    {
        this.buildingsRepository = buildingsRepository;
        this.mapper = mapper;
        this.imageService = imageService;
    }

    public async Task<OperationResult<Guid>> Create(Building building)
    {
        var checkResult = await CheckBuilding(building).ConfigureAwait(false);
        if (checkResult.IsFailure)
            return OperationResult<Guid>.Failure(checkResult.ApiError);

        var createFloorsOperation = await CreateFloors(building.Id, building.Floors).ConfigureAwait(false);
        if (createFloorsOperation.IsFailure)
            return OperationResult<Guid>.Failure(createFloorsOperation.ApiError);

        var buildingEntity = mapper.Map<BuildingEntity>(building);
        buildingEntity = await buildingsRepository.AddAsync(buildingEntity).ConfigureAwait(false);
        await buildingsRepository.SaveChangesAsync().ConfigureAwait(false);
        return OperationResult<Guid>.Success(buildingEntity.Id);
    }

    public async Task<OperationResult> Patch(Building building)
    {
        var check = await CheckBuilding(building, checkExist: false).ConfigureAwait(false);
        if (check.IsFailure)
            return check;
        
        var buildingEntity = mapper.Map<BuildingEntity>(building);
        
        buildingsRepository.Update(buildingEntity);
        await buildingsRepository.SaveChangesAsync().ConfigureAwait(false);
        
        return OperationResult.Success();
    }

    public async Task<OperationResult> PatchRange(List<Building> buildings)
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

    public async Task<OperationResult> Delete(Guid id)
    {
        await buildingsRepository.RemoveByIdAsync(id).ConfigureAwait(false);
        await buildingsRepository.SaveChangesAsync().ConfigureAwait(false);
        
        return OperationResult.Success();
    }

    public async Task<OperationResult<List<Building>>> GetAllByFilter(BuildingFilter filter)
    {
        var buildingEntities = await buildingsRepository.GetAllByFilterAsync(filter).ConfigureAwait(false);
    
        var buildingTasks = buildingEntities.Select(async entity => await GetBuildingByEntity(entity).ConfigureAwait(false)).ToList();
        var buildingResults = await Task.WhenAll(buildingTasks);

        if (buildingResults.Any(r => r.IsFailure))
        {
            return OperationResult<List<Building>>.Failure(buildingResults.First(r => r.IsFailure).ApiError);
        }

        return OperationResult<List<Building>>.Success(buildingResults.Select(r => r.Data).ToList());
    }

    public async Task<OperationResult<Building>> GetById(Guid id)
    {
        var buildingEntity = await buildingsRepository.GetByIdAsync(id).ConfigureAwait(false);
        if (buildingEntity is null)
            return OperationResult<Building>.Failure(BuildingsApiErrors.BuildingWithIdNotFoundError(id));

        return await GetBuildingByEntity(buildingEntity).ConfigureAwait(false);
    }

    private async Task<OperationResult<List<FloorInfo>>> GetFloors(BuildingEntity building)
    {
        var tasks = new List<Task<OperationResult<FloorInfo>>>();

        for (var i = 1; i <= building.FloorNumbers; i++)
        {
            tasks.Add(GetFloorInfo(building, i));
        }
        
        var results = await Task.WhenAll(tasks);
        
        foreach (var result in results)
        {
            if (result.IsFailure) return OperationResult<List<FloorInfo>>.Failure(result.ApiError);
        }
        
        return OperationResult<List<FloorInfo>>.Success(results.Select(r => r.Data).ToList());
    }

    private async Task<OperationResult<FloorInfo>> GetFloorInfo(BuildingEntity building, int floorNumber)
    {
        var title = GetTittleForFloorImage(floorNumber);
        var filter = new ImageFilter
        {
            ObjectId = building.Id,
            Title = title
        };

        var floorImg = await imageService.GetAllByFilter(filter).ConfigureAwait(false);

        if (floorImg.IsFailure)
            return OperationResult<FloorInfo>.Failure(floorImg.ApiError);

        if (floorImg.Data.Count == 0)
            return OperationResult<FloorInfo>.Failure(BuildingsApiErrors.ImageWithFloorIdNotFoundError(building.Id, floorNumber));

        return OperationResult<FloorInfo>.Success(new FloorInfo(floorNumber, floorImg.Data.First().Link));
    }

    private async Task<OperationResult> CreateFloors(Guid buildingId, List<FloorInfo> floorInfos)
    {
        var links = floorInfos.Select(x =>
            new ImageLink(Guid.NewGuid(), buildingId, x.ImageLink, GetTittleForFloorImage(x.FloorNumber))).ToList();
        var createOperation = await imageService.CreateRange(links).ConfigureAwait(false);
        
        return createOperation.IsSuccess 
            ? OperationResult.Success() 
            : OperationResult.Failure(createOperation.ApiError);
    }

    private async Task<OperationResult> CheckBuilding(Building building, bool checkExist = true)
    {
        if (string.IsNullOrWhiteSpace(building.Title))
            return OperationResult.Failure(CommonErrors.EmptyTitleError());
        
        if(building.Floors.Count == 0)
            return OperationResult.Failure(BuildingsApiErrors.NoFloorsError());

        var floors = building.Floors.OrderBy(x => x.FloorNumber).ToList();
        
        for (var i = 0; i < floors.Count; i++)
        {
            var expectedNumber = i + 1;
            if(floors[i].FloorNumber != expectedNumber)
                return OperationResult.Failure(BuildingsApiErrors.WrongFloorNumberError(floors[i].FloorNumber, expectedNumber));
        }
        
        if (!checkExist)
            return OperationResult.Success();

        var isExist = await buildingsRepository.GetByIdAsync(building.Id).ConfigureAwait(false) != null;
        return isExist
            ? OperationResult.Failure(BuildingsApiErrors.BuildingAlreadyExistsError(building.Id))
            : OperationResult.Success();
    }

    private async Task<OperationResult<Building>> GetBuildingByEntity(BuildingEntity buildingEntity)
    {
        var floors = await GetFloors(buildingEntity).ConfigureAwait(false);
        if (floors.IsFailure)
            return OperationResult<Building>.Failure(floors.ApiError);

        var building = new Building
        {
            Id = buildingEntity.Id,
            Title = buildingEntity.Title,
            Floors = floors.Data,
            Description = buildingEntity.Description
        };
        return OperationResult<Building>.Success(building);
    }

    private string GetTittleForFloorImage(int floorNumber)
    {
        return "floor_" + floorNumber;
    }
}