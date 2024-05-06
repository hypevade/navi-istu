namespace Istu.Navigation.Infrastructure.Errors.ExternalRoutesApiErrors;

public class ExternalRoutesApiError
{
    private static string GetUrn(string errorType) => $"urn:external-routes-api-errors:{errorType}";
    
    public static ApiError StartPointOutsideAreaError(double latitude, double longitude)
    {
        return new ApiError(400, $"Начальная точка должна быть в пределах карты. Переданная точка - Широта: {latitude}, Долгота: {longitude}.",
            GetUrn("start-point-outside-area"));
    }
    
    public static ApiError EndPointOutsideAreaError(double latitude, double longitude)
    {
        return new ApiError(400, $"Конечная точка должна быть в пределах карты. Переданная точка - Широта: {latitude}, Долгота: {longitude}.",
            GetUrn("end-point-outside-area"));
    }
}