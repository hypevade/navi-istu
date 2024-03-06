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
    
    public static ApiError TargetObjectIsEqualToSourceError(Guid objectId)
    {
        return new ApiError(400, $"Объект с идентификатором {objectId} является целевым и совпадает с исходным.",
            GetUrn("target-object-is-equal-to-source"));
    }
    
    public static ApiError FloorContainsNoObjectsError(Guid buildingId, int floor)
    {
        return new ApiError(400, $"Этаж {floor} в здании с идентификатором {buildingId} не содержит объектов. Возможно вам стоит добавить их.",
            GetUrn("floor-contains-no-objects"));
    }
    
    public static ApiError FloorContainsNoEdgesError(Guid buildingId, int floor)
    {
        return new ApiError(400, $"Этаж {floor} в здании с идентификатором {buildingId} не содержит ребер между объектами. Возможно вам стоит добавить их.",
            GetUrn("floor-contains-no-edges"));
    }
}