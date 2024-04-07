using AutoMapper;
using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Repositories;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.Errors.RoutesApiErrors;

namespace Istu.Navigation.Domain.Services;

public interface IFloorsService
{
    public Task<OperationResult<List<Floor>>> GetAllByBuilding(Guid buildingId); 
    public Task<OperationResult<Floor>> GetById(Guid buildingId, int floorNumber);
    public Task<OperationResult> Create(List<Floor> floors);
    public Task<OperationResult> Patch(List<Floor> floors);
    public Task<OperationResult> Delete(List<(Guid buildingId, int floor)> floorIds);
}

public class FloorsService : IFloorsService
{
    private readonly IFloorsRepository floorsRepository;
    private readonly IBuildingObjectsService buildingObjectsService;
    private readonly IEdgesService edgesService;
    private readonly IImageService imageService;
    private readonly IBuildingsService buildingsService;
    private readonly IMapper mapper;

    public FloorsService(IFloorsRepository floorsRepository,
        IMapper mapper,
        IBuildingObjectsService buildingObjectsService,
        IEdgesService edgesService,
        IImageService imageService,
        IBuildingsService buildingsService)
    {
        this.mapper = mapper;
        this.buildingObjectsService = buildingObjectsService;
        this.edgesService = edgesService;
        this.imageService = imageService;
        this.buildingsService = buildingsService;
        this.floorsRepository = floorsRepository;
    }

    public async Task<OperationResult<List<Floor>>> GetAllByBuilding(Guid buildingId)
    {
        //TODO: Поисследовать, как можно оптимизировать запрос всех этажей
        var floorEntities = await floorsRepository.GetAllByBuilding(buildingId).ConfigureAwait(false);
        var floors = new List<Floor>();
        
        foreach (var floorEntity in floorEntities)
        {
            var getFloor = await GetById(floorEntity.BuildingId, floorEntity.FloorNumber).ConfigureAwait(false);
            if (getFloor.IsFailure)
                return OperationResult<List<Floor>>.Failure(getFloor.ApiError);
            floors.Add(getFloor.Data);
        }

        return OperationResult<List<Floor>>.Success(floors);
    }

    public async Task<OperationResult<Floor>> GetById(Guid buildingId, int floorNumber)
    {
        var floorEntity = await floorsRepository.GetById(buildingId, floorNumber).ConfigureAwait(false);
        if (floorEntity is null)
            return OperationResult<Floor>.Failure(
                BuildingsErrors.FloorWithBuildingAndFloorNumberIdNotFoundError(buildingId, floorNumber));
        
        var getBuildingObjects = await buildingObjectsService
            .GetAllByFloor(buildingId, floorNumber)
            .ConfigureAwait(false);
        if (getBuildingObjects.IsFailure)
            return OperationResult<Floor>.Failure(getBuildingObjects.ApiError);
        
        var getEdges = await edgesService.GetAllByFloor(buildingId, floorNumber).ConfigureAwait(false);
        if (getEdges.IsFailure)
            return OperationResult<Floor>.Failure(getEdges.ApiError);
        
        var getImageLink = await imageService.GetById(floorEntity.ImageId).ConfigureAwait(false);
        if (getImageLink.IsFailure)
            return OperationResult<Floor>.Failure(getImageLink.ApiError);

        var floor = Floor.FromEntity(floorEntity, getBuildingObjects.Data, getEdges.Data, getImageLink.Data);
        return OperationResult<Floor>.Success(floor);
    }

    public async Task<OperationResult> Create(List<Floor> floors)
    {
        foreach (var floor in floors)
        {
            var check = await CheckFloor(floor).ConfigureAwait(false);
            if(check.IsFailure)
                return check;
        }
        
        var floorEntities = mapper.Map<List<FloorEntity>>(floors);
        
        await floorsRepository.AddRangeAsync(floorEntities).ConfigureAwait(false);
        await floorsRepository.SaveChangesAsync().ConfigureAwait(false);
        return OperationResult.Success();
    }

    public async Task<OperationResult> Patch(List<Floor> floors)
    {
        foreach (var floor in floors)
        {
            var check = await CheckFloor(floor, checkExist: false).ConfigureAwait(false);
            if(check.IsFailure)
                return check;
        }

        var floorEntities = mapper.Map<List<FloorEntity>>(floors);
        floorsRepository.UpdateRange(floorEntities);
        await floorsRepository.SaveChangesAsync().ConfigureAwait(false);
        return OperationResult.Success();
    }
    
    //TODO: Продумать удаление этажа

    public Task<OperationResult> Delete(List<(Guid buildingId, int floor)> floorIds)
    {
        throw new NotImplementedException();
    }

    private async Task<OperationResult> CheckFloor(Floor floor, bool checkExist = true)
    {
        if (floor.FloorNumber < 0)
            return OperationResult.Failure(BuildingsErrors.NegativeFloorNumbersError());
        var tryGetBuilding = await buildingsService.GetById(floor.BuildingId).ConfigureAwait(false);

        if (tryGetBuilding.IsFailure)
            return OperationResult.Failure(tryGetBuilding.ApiError);

        if (!checkExist)
            return OperationResult.Success();

        var tryGetFloor = await floorsRepository.GetById(floor.BuildingId, floor.FloorNumber).ConfigureAwait(false);
        return tryGetFloor is null
            ? OperationResult.Success()
            : OperationResult.Failure(
                BuildingsErrors.FloorWithIdAlreadyExistError(floor.BuildingId, floor.FloorNumber));
    }
}