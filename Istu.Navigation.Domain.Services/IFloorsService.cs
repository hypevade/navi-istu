using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Repositories;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.Errors;
using Istu.Navigation.Infrastructure.Errors.Errors.RoutesApiErrors;

namespace Istu.Navigation.Domain.Services;

public interface IFloorsService
{
    public Task<OperationResult<FloorInfo>> GetFloorInfo(Guid buildingId, int floorNumber);
    public Task<OperationResult<Guid>> CreateFloor(Guid buildingId, string imageLink, int? floorNumber = null);
    public Task<OperationResult> DeleteFloor(Guid buildingId, int floorNumber);
    public Task<OperationResult<List<FloorInfo>>> GetFloorInfosByBuilding(Guid buildingId);
}


public class FloorsService : IFloorsService
{
    private readonly IImageService imageService;
    private readonly IBuildingsRepository buildingsRepository;
    private readonly IFloorsRepository floorsRepository;

    public FloorsService(IImageService imageService,
        IFloorsRepository floorsRepository,
        IBuildingsRepository buildingsRepository)
    {
        this.imageService = imageService;
        this.floorsRepository = floorsRepository;
        this.buildingsRepository = buildingsRepository;
    }

    public async Task<OperationResult> DeleteFloor(Guid buildingId, int floorNumber)
    {
        var floor = await floorsRepository.GetByBuildingIdAsync(buildingId, floorNumber).ConfigureAwait(false);
        if (floor is null)
            return OperationResult.Success();

        await floorsRepository.RemoveByIdAsync(floor.Id).ConfigureAwait(false);
        await floorsRepository.SaveChangesAsync().ConfigureAwait(false);
        return OperationResult.Success();
    }

    public async Task<OperationResult<List<FloorInfo>>> GetFloorInfosByBuilding(Guid buildingId)
    {
        var floors = await floorsRepository.GetAllByBuildingIdAsync(buildingId).ConfigureAwait(false);
        var floorInfos = new List<FloorInfo>();
        foreach (var floorEntity in floors)
        {
            var getFloorInfo = await GetFloorInfo(buildingId, floorEntity.FloorNumber).ConfigureAwait(false);
            if (getFloorInfo.IsFailure)
                return OperationResult<List<FloorInfo>>.Failure(getFloorInfo.ApiError);
            floorInfos.Add(getFloorInfo.Data);
        }

        return OperationResult<List<FloorInfo>>.Success(floorInfos);
    }

    public async Task<OperationResult<Guid>> CreateFloor(Guid buildingId, string imageLink, int? floorNumber = null)
    {
        var checkFloor = await CheckFloor(buildingId, floorNumber).ConfigureAwait(false);
        if (checkFloor.IsFailure)
            return OperationResult<Guid>.Failure(checkFloor.ApiError);

        var floorsByBuilding = await floorsRepository.GetAllByBuildingIdAsync(buildingId).ConfigureAwait(false);
        if (floorNumber is not null && floorsByBuilding.Any(x => x.FloorNumber == floorNumber.Value))
            return OperationResult<Guid>.Failure(
                BuildingsApiErrors.FloorWithBuildingAndFloorNumberAlreadyExistsError(buildingId, floorNumber.Value));


        if (floorsByBuilding.Any())
            floorNumber ??= floorsByBuilding.Max(x => x.FloorNumber) + 1;
        else
            floorNumber ??= 1;
        
        var floorEntity = new FloorEntity
        {
            Id = Guid.NewGuid(),
            BuildingId = buildingId,
            FloorNumber = floorNumber.Value,
        };

        await floorsRepository.AddAsync(floorEntity).ConfigureAwait(false);
        await floorsRepository.SaveChangesAsync().ConfigureAwait(false);
        var link = new ImageLink(Guid.NewGuid(), floorEntity.Id, imageLink, $"floor_{floorNumber}");
        var createLink = await imageService.Create(link).ConfigureAwait(false);
        if (createLink.IsFailure)
        {
            return createLink.ApiError.Urn == ImagesApiErrors.ImageWithEmptyLinkError().Urn
                ? OperationResult<Guid>.Failure(createLink.ApiError)
                : OperationResult<Guid>.Failure(CommonErrors.InternalServerError());
        }
        return OperationResult<Guid>.Success(floorEntity.Id);
    }

    public async Task<OperationResult<FloorInfo>> GetFloorInfo(Guid buildingId, int floorNumber)
    {
        var getFloor = await floorsRepository.GetByBuildingIdAsync(buildingId, floorNumber).ConfigureAwait(false);
        if (getFloor is null)
            return OperationResult<FloorInfo>.Failure(
                BuildingsApiErrors.FloorWithBuildingAndFloorNumberNotFoundError(buildingId, floorNumber));
        return await GetFloorInfoByEntity(getFloor).ConfigureAwait(false);
    }


    private async Task<OperationResult<FloorInfo>> GetFloorInfoByEntity(FloorEntity floorEntity)
    {
        var images = await imageService.GetAllByObjectId(floorEntity.Id).ConfigureAwait(false);
        if (images.IsFailure)
            return OperationResult<FloorInfo>.Failure(images.ApiError);
        var image = images.Data.FirstOrDefault();
        if (image is null)
            return OperationResult<FloorInfo>.Failure(
                BuildingsApiErrors.ImageWithFloorIdNotFoundError(floorEntity.BuildingId, floorEntity.FloorNumber));
        return OperationResult<FloorInfo>.Success(new FloorInfo(floorEntity.Id, floorEntity.FloorNumber, image.Link));
    }

    private async Task<OperationResult> CheckFloor(Guid buildingId, int? floorNumber = null)
    {
        if (floorNumber is < 1)
            return OperationResult.Failure(
                BuildingsApiErrors.FloorNumberLessThanMinFloorError(floorNumber.Value));

        var building = await buildingsRepository.GetByIdAsync(buildingId).ConfigureAwait(false);
        if (building is null)
            return OperationResult.Failure(BuildingsApiErrors.BuildingWithIdNotFoundError(buildingId));
        return OperationResult.Success();
    }
}