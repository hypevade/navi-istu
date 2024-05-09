using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Api.Filters;
using Istu.Navigation.Api.Paths;
using Istu.Navigation.Domain.Services.Buildings;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route(ApiRoutes.Images.ImagesApi)]
public class ImagesController(IImageService imageService) : ControllerBase
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
}