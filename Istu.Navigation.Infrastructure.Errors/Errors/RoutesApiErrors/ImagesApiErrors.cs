namespace Istu.Navigation.Infrastructure.Errors.Errors.RoutesApiErrors;

public class ImagesApiErrors
{
    private static string GetUrn(string errorType) => $"urn:images-api-errors:{errorType}";
    
    public static ApiError ImageWithEmptyLinkError()
    {
        return new ApiError(400, $"Ссылка на изображение не может быть пустой.",
            GetUrn("image-with-empty-link"));
    }
}