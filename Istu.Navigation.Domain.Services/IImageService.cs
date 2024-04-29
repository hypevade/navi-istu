using AutoMapper;
using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Repositories;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.Errors.RoutesApiErrors;

namespace Istu.Navigation.Domain.Services;
public interface IImageService
{
    public Task<OperationResult<ImageLink>> GetById(Guid imageId);
    public Task<OperationResult<List<ImageLink>>> GetAllByFilter(ImageFilter filter);
    public Task<OperationResult<List<ImageLink>>> GetAllByObjectId(Guid objectId);
    public Task<OperationResult<Guid>> Create(ImageLink image);
    public Task<OperationResult<List<Guid>>> CreateRange(List<ImageLink> image);
    public Task<OperationResult> Delete(Guid imageId);
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

    public async Task<OperationResult<List<ImageLink>>> GetAllByFilter(ImageFilter filter)
    {
        var images = await imageRepository.GetAllByFilterAsync(filter).ConfigureAwait(false);
        return OperationResult<List<ImageLink>>.Success(mapper.Map<List<ImageLink>>(images));
    }

    public async Task<OperationResult<List<ImageLink>>> GetAllByObjectId(Guid objectId)
    {
        var images = await imageRepository.GetAllByObjectId(objectId).ConfigureAwait(false);
        return OperationResult<List<ImageLink>>.Success(mapper.Map<List<ImageLink>>(images));
    }
    
    public async Task<OperationResult<ImageLink>> GetByFloorId(Guid buildingId, int floorNumber)
    {
        var res = await imageRepository.FindAsync(x => x.ObjectId == buildingId && x.Title == "floor_" + floorNumber);
        if (res.Count == 0)
            return OperationResult<ImageLink>.Failure(
                BuildingsErrors.ImageWithFloorIdNotFoundError(buildingId, floorNumber));

        return OperationResult<ImageLink>.Success(mapper.Map<ImageLink>(res.First()));
        
    }

    public async Task<OperationResult<Guid>> Create(ImageLink image)
    {
        if(string.IsNullOrEmpty(image.Link.Trim()))
            return OperationResult<Guid>.Failure(ImagesApiErrors.ImageWithEmptyLinkError());
        var imageEntity = mapper.Map<ImageLinkEntity>(image);
        var linkEntity = await imageRepository.AddAsync(imageEntity).ConfigureAwait(false);
        await imageRepository.SaveChangesAsync().ConfigureAwait(false);
        return OperationResult<Guid>.Success(linkEntity.Id);
    }

    public async Task<OperationResult<List<Guid>>> CreateRange(List<ImageLink> images)
    {
        var imageEntities = mapper.Map<List<ImageLinkEntity>>(images);
        var linkEntities = await imageRepository.AddRangeAsync(imageEntities).ConfigureAwait(false);
        await imageRepository.SaveChangesAsync().ConfigureAwait(false);
        
        var linkIds = linkEntities.Select(x => x.Id).ToList();
        return OperationResult<List<Guid>>.Success(linkIds);
    }

    public async Task<OperationResult> Delete(Guid imageId)
    {
        await imageRepository.RemoveByIdAsync(imageId).ConfigureAwait(false);
        await imageRepository.SaveChangesAsync().ConfigureAwait(false);
        return OperationResult.Success();
    }
}