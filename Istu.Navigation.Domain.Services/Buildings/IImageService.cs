using AutoMapper;
using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Repositories.Buildings;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.RoutesApiErrors;
using Microsoft.AspNetCore.Http;
using FileInfo = Istu.Navigation.Domain.Models.BuildingRoutes.FileInfo;


namespace Istu.Navigation.Domain.Services.Buildings;
public interface IImageService
{
    public Task<OperationResult<Guid>> CreateAsync(IFormFile file, Guid objectId);
    public Task<OperationResult<FileInfo>> GetImageByIdAsync(Guid imageId);
    public Task<OperationResult<List<ImageInfo>>> GetInfosByFilterAsync(ImageFilter filter);
    public Task<OperationResult<List<ImageInfo>>> GetInfosByObjectIdAsync(Guid objectId);
    public Task<OperationResult> DeleteAsync(Guid imageId);
}

public class ImageService(IImageRepository repository, IImageStorage storage, IMapper mapper) : IImageService
{
    public async Task<OperationResult<FileInfo>> GetImageByIdAsync(Guid imageId)
    {
        var image = await repository.GetByIdAsync(imageId).ConfigureAwait(false);
        if (image is null)
            return OperationResult<FileInfo>.Failure(ImagesApiErrors.ImageWithIdNotFoundError(imageId));
        return await storage.DownloadAsync(image.Title).ConfigureAwait(false);
    }

    public async Task<OperationResult<List<ImageInfo>>> GetInfosByFilterAsync(ImageFilter filter)
    {
        var images = await repository.GetAllByFilterAsync(filter).ConfigureAwait(false);
        return OperationResult<List<ImageInfo>>.Success(mapper.Map<List<ImageInfo>>(images));
    }

    public async Task<OperationResult<List<ImageInfo>>> GetInfosByObjectIdAsync(Guid objectId)
    {
        var images = await repository.GetAllByObjectId(objectId).ConfigureAwait(false);
        return OperationResult<List<ImageInfo>>.Success(mapper.Map<List<ImageInfo>>(images));
    }

    public async Task<OperationResult<Guid>> CreateAsync(IFormFile file, Guid objectId)
    {
        var imageId = Guid.NewGuid();
        var filename = CreateUniqueFileName(file.FileName, imageId);
        var image = new ImageInfoEntity
        {
            Id = imageId,
            Title = filename,
            ObjectId = objectId
        };
        var addAsync = await repository.AddAsync(image).ConfigureAwait(false);
        await repository.SaveChangesAsync().ConfigureAwait(false);
        var uploadOperation = await storage.UploadAsync(file, filename).ConfigureAwait(false);
        return uploadOperation.IsSuccess
            ? OperationResult<Guid>.Success(addAsync.Id)
            : OperationResult<Guid>.Failure(uploadOperation.ApiError);
    }

    private string CreateUniqueFileName(string originalFileName, Guid id)
    {
        var extension = Path.GetExtension(originalFileName);
        return $"{id}{extension}";
    }

    public async Task<OperationResult> DeleteAsync(Guid imageId)
    {
        await repository.RemoveByIdAsync(imageId).ConfigureAwait(false);
        await repository.SaveChangesAsync().ConfigureAwait(false);
        return OperationResult.Success();
    }
}