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
[Route(ApiRoutes.Images.ImagesApi)]
[AuthorizationFilter(UserRole.User)]
public class ImagesController(IImageService imageService, IMapper mapper) : ControllerBase
{

    [MaxFileSize(10485760)]
    [HttpPost(ApiRoutes.Images.UploadPart)]
    
    public async Task<ActionResult<Guid>> Create(IFormFile file, Guid objectId)
    {
        var uploadOperation = await imageService.CreateAsync(file, objectId).ConfigureAwait(false);
        if (uploadOperation.IsFailure)
        {
            var apiError = uploadOperation.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        return Ok(uploadOperation.Data);
    }

    [HttpGet(ApiRoutes.Images.DownloadPart)]
    public async Task<ActionResult<byte[]>> Download(Guid imageId)
    {
        var downloadOperation = await imageService.GetImageByIdAsync(imageId).ConfigureAwait(false);
        if (downloadOperation.IsFailure)
        {
            var apiError = downloadOperation.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        var fileInfo = downloadOperation.Data;

        return File(fileInfo.Content, "application/octet-stream", fileInfo.Name);
    }
    
    [HttpGet(ApiRoutes.Images.GetPart)]
    public async Task<ActionResult<ImageInfosResponse>> GetByObject(Guid objectId)
    {
        var downloadOperation = await imageService.GetInfosByObjectIdAsync(objectId).ConfigureAwait(false);
        if (downloadOperation.IsFailure)
        {
            var apiError = downloadOperation.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        var imageInfos = mapper.Map<List<ImageInfoDto>>(downloadOperation.Data);
        return Ok(new ImageInfosResponse {Images = imageInfos});
    }
    
}