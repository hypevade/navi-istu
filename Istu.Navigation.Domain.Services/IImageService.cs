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

    public Task<OperationResult<List<ImageLink>>> GetAllByObjectId(Guid objectId)
    {
        throw new NotImplementedException();
    }
}