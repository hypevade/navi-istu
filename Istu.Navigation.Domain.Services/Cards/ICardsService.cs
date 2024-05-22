using System.Globalization;
using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Models.Cards;
using Istu.Navigation.Domain.Services.Buildings;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.RoutesApiErrors;
using Microsoft.Extensions.Logging;

namespace Istu.Navigation.Domain.Services.Cards;

public interface ICardsService
{
    public Task<OperationResult<Card>> GetCard(Guid objectId);
}

public class CardsService : ICardsService
{
    private readonly IBuildingsService buildingsService;
    private readonly IBuildingObjectsService buildingObjectsService;
    private readonly IImageService imageService;
    private readonly ILogger<CardsService> logger;
    

    public CardsService(IImageService imageService, IBuildingsService buildingsService, ILogger<CardsService> logger,
        IBuildingObjectsService buildingObjectsService)
    {
        this.imageService = imageService;
        this.buildingsService = buildingsService;
        this.logger = logger;
        this.buildingObjectsService = buildingObjectsService;
    }

    public async Task<OperationResult<Card>> GetCard(Guid objectId)
    {
        var getBuildingCardOperation = await GetBuildingCard(objectId).ConfigureAwait(false);
        if (getBuildingCardOperation.IsFailure && getBuildingCardOperation.ApiError.Urn !=
            BuildingsApiErrors.BuildingWithIdNotFoundError(objectId).Urn)
            return OperationResult<Card>.Failure(getBuildingCardOperation.ApiError);
        
        if(getBuildingCardOperation.IsSuccess)
            return OperationResult<Card>.Success(getBuildingCardOperation.Data);

        var getObjectCardOperation = await GetObjectCard(objectId).ConfigureAwait(false);
        if(getObjectCardOperation.IsFailure)
            return OperationResult<Card>.Failure(getObjectCardOperation.ApiError);

        return OperationResult<Card>.Success(getObjectCardOperation.Data);
    }
    
    private async Task<OperationResult<Card>> GetObjectCard(Guid buildingObjectId)
    {
        var getObjectOperation = await buildingObjectsService.GetByIdAsync(buildingObjectId).ConfigureAwait(false);
        if (getObjectOperation.IsFailure)
            return OperationResult<Card>.Failure(getObjectOperation.ApiError);
        
        var images = await GetImageIds(buildingObjectId).ConfigureAwait(false);

        var buildingObject = getObjectOperation.Data;
        var getBuildingOperation = await buildingsService.GetByIdAsync(buildingObject.BuildingId).ConfigureAwait(false);
        if (getBuildingOperation.IsFailure)
            return OperationResult<Card>.Failure(getBuildingOperation.ApiError);
        var building = getBuildingOperation.Data;
        var properties = new Dictionary<string, string>
        {
            { "floorNumber", buildingObject.Floor.ToString() },
            { "buildingTitle", building.Title },
            { "buildingAddress", building.Address },
            { "buildingId", buildingObject.Id.ToString() }
        };
        if (string.IsNullOrEmpty(buildingObject.Title))
            buildingObject.Title = buildingObject.Type.GetRussianName();

        var card = new Card
        {
            Address = GetObjectAddress(buildingObject, building),
            ContentType = ContentType.Object,
            Description = buildingObject.Description,
            Title = buildingObject.Title,
            ObjectId = buildingObjectId,
            Properties = properties,
            ImageIds = images
        };
        return OperationResult<Card>.Success(card);
    }
    
    private string GetObjectAddress(BuildingObject buildingObject, Building building)
    {
        return $"{building.Title}, {buildingObject.Floor} этаж";
    }

    private async Task<OperationResult<Card>> GetBuildingCard(Guid buildingId)
    {
        var getBuildingOperation = await buildingsService.GetByIdAsync(buildingId).ConfigureAwait(false);
        if (getBuildingOperation.IsFailure)
            return OperationResult<Card>.Failure(getBuildingOperation.ApiError);
        
        var images = await GetImageIds(buildingId).ConfigureAwait(false);

        var building = getBuildingOperation.Data;
        var properties = new Dictionary<string, string>()
        {
            { "address", building.Address },
            { "numberOfFloors", building.FloorNumbers.ToString() },
            { "Latitude", building.Latitude.ToString(CultureInfo.CurrentCulture) },
            { "Longitude", building.Longitude.ToString(CultureInfo.CurrentCulture) },
        };

        var card = new Card
        {
            Address = building.Address,
            ContentType = ContentType.Building,
            Description = building.Description,
            Title = building.Title,
            ObjectId = buildingId,
            Properties = properties,
            ImageIds = images
        };
        return OperationResult<Card>.Success(card);
    }

    private async Task<List<Guid>> GetImageIds(Guid objectId)
    {
        var getImagesOperation = await imageService.GetInfosByObjectIdAsync(objectId).ConfigureAwait(false);
        if (getImagesOperation.IsFailure)
        {
            logger.LogError(
                $"Failed to get images for object with id {objectId}, error: {getImagesOperation.ApiError}, will return empty list");
            return new();
        }

        return getImagesOperation.Data.Select(x => x.Id).ToList();
    }
}