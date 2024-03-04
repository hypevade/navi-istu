namespace Istu.Navigation.Infrastructure.Errors.Errors.RoutesApiErrors;

public class BuildingRoutesErrors : ErrorBase
{
    protected new string Nid = CommonConstans.RoutesApiNid;

    public ApiError BuildingObjectNotFoundError(Guid buildingObjectId)
    {
        return new ApiError(404, $"Объект с идентификатором {buildingObjectId} не найден.",
            GetUrn("building-object-not-found"));
    }

    public ApiError BuildingEdgeNotFoundError(Guid fromObjectId, Guid toObjectId)
    {
        return new ApiError(404, $"Ребро между объектами с идентификаторами {fromObjectId} и {toObjectId} не найдено.",
            GetUrn("building-edge-not-found"));
    }
    
    public static ApiError BuildingRouteNotFoundError(Guid fromObjectId, Guid toObjectId)
    {
        return new ApiError(404, $"Путь между объектами с идентификаторами {fromObjectId} и {toObjectId} не найден.",
            GetUrn("building-route-not-found"));
    }
}