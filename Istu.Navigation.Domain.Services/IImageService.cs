using AutoMapper;
using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Repositories;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.Errors.RoutesApiErrors;

namespace Istu.Navigation.Domain.Services;

public interface IImageService
{
    public Task<OperationResult<ImageLink>> GetById(Guid imageId);
    public Task<OperationResult<List<ImageLink>>> GetAllByObjectId(Guid objectId);
    public Task<OperationResult<ImageLink>> GetByFloorId(Guid buildingId, int floorNumber);
}

public class ImageService : IImageService
{
    private readonly IImageRepository imageRepository;
    private readonly IMapper mapper;

    public ImageService(IImageRepository imageRepository, IMapper mapper)
    {
        this.imageRepository = imageRepository;
        this.mapper = mapper;
    }

    public async Task<OperationResult<ImageLink>> GetById(Guid imageId)
    {
        var image = await imageRepository.GetByIdAsync(imageId).ConfigureAwait(false);
        if (image is null)
            return OperationResult<ImageLink>.Failure(BuildingsErrors.ImageWithIdNotFoundError(imageId));
        return OperationResult<ImageLink>.Success(mapper.Map<ImageLink>(image));
    }

    public async Task<OperationResult<List<ImageLink>>> GetAllByObjectId(Guid objectId)
    {
        throw new NotImplementedException();
    }
    
    public async Task<OperationResult<ImageLink>> GetByFloorId(Guid buildingId, int floorNumber)
    {
        var res = await imageRepository.FindAsync(x => x.ObjectId == buildingId && x.Title == "floor_" + floorNumber);
        if (res.Count == 0)
            return OperationResult<ImageLink>.Failure(
                BuildingsErrors.ImageWithFloorIdNotFoundError(buildingId, floorNumber));

        return OperationResult<ImageLink>.Success(mapper.Map<ImageLink>(res.First()));
        
    }
}