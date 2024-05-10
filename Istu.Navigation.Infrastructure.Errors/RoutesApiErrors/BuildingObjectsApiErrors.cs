namespace Istu.Navigation.Infrastructure.Errors.RoutesApiErrors;

public class BuildingObjectsApiErrors
{
    private static string GetUrn(string errorType) => $"urn:building-objects-api-errors:{errorType}";
    
    public static ApiError BuildingObjectNotFoundError(Guid buildingObjectId)
    {
        return new ApiError(404, $"Объект с идентификатором '{buildingObjectId}' не найден.",
            GetUrn("building-object-not-found"));
    }
    
    public static ApiError InvalidCoordinatesError(double x, double y)
    {
        return new ApiError(400, 
            $"Координаты объекта должны быть в диапазоне от 0 до 1. Но у вас координаты ({x}, {y}).",
            GetUrn("building-objects-with-invalid-coordinates"));
    }
    
    public static ApiError BuildingObjectAlreadyExistsError(Guid buildingId)
    {
        return new ApiError(400,
            $"Объект с идентификатором '{buildingId}' уже существует.",
            GetUrn("building-object-already-exists"));
    }
}