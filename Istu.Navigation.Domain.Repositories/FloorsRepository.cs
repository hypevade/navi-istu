using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Infrastructure.EF;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.Errors.RoutesApiErrors;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Domain.Repositories;

public class FloorsRepository : IFloorsRepository
{
    private readonly BuildingsDbContext dbContext;
    private readonly IEdgesRepository edgesRepository;
    private readonly IBuildingObjectsRepository buildingObjectsRepository;
    private readonly IImageRepository imageRepository;

    public FloorsRepository(BuildingsDbContext dbContext,
        IEdgesRepository edgesRepository,
        IBuildingObjectsRepository buildingObjectsRepository,
        IImageRepository imageRepository)
    {
        this.dbContext = dbContext;
        this.edgesRepository = edgesRepository;
        this.buildingObjectsRepository = buildingObjectsRepository;
        this.imageRepository = imageRepository;
    }

    public async Task<OperationResult<Floor>> GetById(Guid buildingId, int floorNumber)
    {
        var floor = await dbContext.Floors
            .FirstOrDefaultAsync(x => x.BuildingId == buildingId && x.FloorNumber == floorNumber).ConfigureAwait(false);
        if (floor is null)
            return OperationResult<Floor>.Failure(
                BuildingRoutesErrors.FloorWithBuildingAndFloorNumberIdNotFoundError(buildingId, floorNumber));
        
        var result = await GetFloorByEntity(floor).ConfigureAwait(false);
        return result.IsFailure
            ? OperationResult<Floor>.Failure(result.ApiError)
            : OperationResult<Floor>.Success(result.Data);
    }

    public async Task<OperationResult<List<Floor>>> GetAllByBuilding(Guid buildingId)
    {
        var floors = await dbContext.Floors
            .Where(x => x.BuildingId == buildingId)
            .ToListAsync().ConfigureAwait(false);
        if (!floors.Any())
            return OperationResult<List<Floor>>.Failure(
                BuildingRoutesErrors.FloorsWithBuildingIdNotFoundError(buildingId));
        
        var result = new List<Floor>();
        foreach (var floorEntity in floors)
        {
            var getFloor = await GetFloorByEntity(floorEntity).ConfigureAwait(false);
            if (getFloor.IsFailure)
                return OperationResult<List<Floor>>.Failure(getFloor.ApiError);
            result.Add(getFloor.Data);
        }

        return OperationResult<List<Floor>>.Success(result);
    }

    private async Task<OperationResult<Floor>> GetFloorByEntity(FloorEntity floorEntity)
    {
        var buildingId = floorEntity.BuildingId;
        var floorNumber = floorEntity.FloorNumber;
        var getEdges = await edgesRepository.GetAllByFloor(buildingId, floorNumber)
            .ConfigureAwait(false);
        if (getEdges.IsFailure)
            return OperationResult<Floor>.Failure(getEdges.ApiError);

        var getObjects = await buildingObjectsRepository.GetAllByFloor(buildingId, floorNumber).ConfigureAwait(false);
        if (getObjects.IsFailure)
            return OperationResult<Floor>.Failure(getObjects.ApiError);
        
        var imageLink = await imageRepository.GetById(floorEntity.ImageId).ConfigureAwait(false);
        if (imageLink.IsFailure)
            return OperationResult<Floor>.Failure(imageLink.ApiError);
        
        var floor = new Floor(buildingId, floorNumber, getObjects.Data, getEdges.Data, imageLink.Data);
        return OperationResult<Floor>.Success(floor);
    }
}