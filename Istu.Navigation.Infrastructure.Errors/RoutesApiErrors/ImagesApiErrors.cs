namespace Istu.Navigation.Infrastructure.Errors.RoutesApiErrors;

public class ImagesApiErrors
{
    private static string GetUrn(string errorType) => $"urn:images-api-errors:{errorType}";
    
    public static ApiError ImageWithIdNotFoundError(Guid imageId)
    {
        return new ApiError(404, $"Изображение с идентификатором '{imageId}' не найдено.",
            GetUrn("image-with-id-not-found"));
    }
    
    public static ApiError ImageWithEmptyLinkError()
    {
        return new ApiError(400, $"Ссылка на изображение не может быть пустой.",
            GetUrn("image-with-empty-link"));
    }
    
    public static ApiError EmptyImageError()
    {
        return new ApiError(400, $"Изображение не может быть пустым.",
            GetUrn("empty-image"));
    }
    
    public static ApiError ImageNotFoundError()
    {
        return new ApiError(400, $"Изображение не найдено.",
            GetUrn("image-not-found"));
    }
    public static ApiError NotImageFileError()
    {
        return new ApiError(400, $"Переданный файл не является изображением.",
            GetUrn("not-image-file"));
    }
}