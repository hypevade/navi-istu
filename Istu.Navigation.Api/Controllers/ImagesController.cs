using AutoMapper;
using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Api.Filters;
using Istu.Navigation.Api.Paths;
using Istu.Navigation.Domain.Models.Users;
using Istu.Navigation.Domain.Services.Buildings;
using Istu.Navigation.Public.Models.Images;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route(ApiRoutes.ImagesRoutes.ImagesApi)]
[AuthorizationFilter(UserRole.User)]
public class ImagesController : ControllerBase
{
    private readonly IImageService imageService;
    private readonly IMapper mapper;

    public ImagesController(IImageService imageService, IMapper mapper)
    {
        this.imageService = imageService;
        this.mapper = mapper;
    }

    [MaxFileSize(10485760)]
    [HttpPost(ApiRoutes.ImagesRoutes.UploadPart)]
    public async Task<ActionResult<Guid>> Create(IFormFile file, Guid objectId)
    {
        var uploadOperation = await imageService.CreateAsync(file, objectId).ConfigureAwait(false);
        
        return uploadOperation.IsFailure
            ? StatusCode(uploadOperation.ApiError.StatusCode, uploadOperation.ApiError.ToErrorDto())
            : Ok(uploadOperation.Data);
    }

    [HttpGet(ApiRoutes.ImagesRoutes.DownloadPart)]
    public async Task<ActionResult<byte[]>> Download(Guid imageId)
    {
        var downloadOperation = await imageService.GetImageByIdAsync(imageId).ConfigureAwait(false);
        
        if (downloadOperation.IsFailure)
            return StatusCode(downloadOperation.ApiError.StatusCode, downloadOperation.ApiError.ToErrorDto());
        
        return File(downloadOperation.Data.Content, "application/octet-stream", downloadOperation.Data.Name);
    }
    
    [HttpGet(ApiRoutes.ImagesRoutes.GetPart)]
    public async Task<ActionResult<ImageInfosResponse>> GetByObject(Guid objectId)
    {
        var downloadOperation = await imageService.GetInfosByObjectIdAsync(objectId).ConfigureAwait(false);
        
        return downloadOperation.IsFailure
            ? StatusCode(downloadOperation.ApiError.StatusCode, downloadOperation.ApiError.ToErrorDto())
            : Ok(new ImageInfosResponse { Images = mapper.Map<List<ImageInfoDto>>(downloadOperation.Data) });
    }
    
}