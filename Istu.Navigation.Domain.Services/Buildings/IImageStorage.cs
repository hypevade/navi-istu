using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.RoutesApiErrors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using FileInfo = Istu.Navigation.Domain.Models.BuildingRoutes.FileInfo;

namespace Istu.Navigation.Domain.Services.Buildings;

public interface IImageStorage
{
    public Task<OperationResult> UploadAsync(IFormFile file, string filename);
    public Task<OperationResult<FileInfo>> DownloadAsync(string title);
}

public class ImageStorage : IImageStorage
{
    private readonly string storagePath;
    
    public ImageStorage(IWebHostEnvironment env)
    {
        storagePath = Path.Combine(env.WebRootPath, "uploads");
        if (!Directory.Exists(storagePath))
            Directory.CreateDirectory(storagePath);
    }
    public async Task<OperationResult> UploadAsync(IFormFile file, string filename)
    {
        if (file.Length == 0)
            OperationResult.Failure(ImagesApiErrors.EmptyImageError());

        if (!IsImageFile(file))
            OperationResult.Failure(ImagesApiErrors.NotImageFileError());

        var filePath = Path.Combine(storagePath, filename);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return OperationResult.Success();
    }

    public async Task<OperationResult<FileInfo>> DownloadAsync(string title)
    {
        var filePath = Path.Combine(storagePath, title);

        if (!File.Exists(filePath))
        {
            return OperationResult<FileInfo>.Failure(ImagesApiErrors.EmptyImageError());
        }

        var fileContents = await File.ReadAllBytesAsync(filePath).ConfigureAwait(false);
        var fileInfo = new FileInfo(title, fileContents);
        return OperationResult<FileInfo>.Success(fileInfo);
    }

    private bool IsImageFile(IFormFile file)
    {
        var provider = new FileExtensionContentTypeProvider();
        return provider.TryGetContentType(file.FileName, out var contentType) &&
               contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
    }
}