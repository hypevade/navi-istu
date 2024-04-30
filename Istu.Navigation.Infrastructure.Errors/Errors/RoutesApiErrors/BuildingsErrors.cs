using Istu.Navigation.Domain.Models.BuildingRoutes;

namespace Istu.Navigation.Infrastructure.Errors.Errors.RoutesApiErrors;

public class BuildingsErrors : ErrorBase
{
    protected new string Nid = CommonConstans.RoutesApiNid;

    public static ApiError BuildingEdgeNotFoundError(Guid fromObjectId, Guid toObjectId)
    {
        return new ApiError(404, $"Ребро между объектами с идентификаторами '{fromObjectId}' и '{toObjectId}' не найдено.",
            GetUrn("building-edge-not-found"));
    }
    
    public static ApiError BuildingRouteNotFoundError(Guid fromObjectId, Guid toObjectId)
    {
        return new ApiError(404, $"Путь между объектами с идентификаторами '{fromObjectId}' и '{toObjectId}' не найден.",
            GetUrn("building-route-not-found"));
    }
    
    public static ApiError BuildingsWithTitleNotFoundError(string title)
    {
        return new ApiError(404, $"Зданий с названием '{title}' не найдено.",
            GetUrn("buildings-with-title-not-found"));
    }
    

    public static ApiError EdgesWithBuildingIdNotFoundError(Guid buildingId)
    {
        return new ApiError(404, $"Ребер в здании с идентификатором '{buildingId}' не найдено.",
            GetUrn("edges-with-building-id-not-found"));
    }
    
    public static ApiError EdgeWithIdNotFoundError(Guid edgeId)
    {
        return new ApiError(404, $"Ребро с идентификатором '{edgeId}' не найдено.",
            GetUrn("edge-with-id-not-found"));
    }
    
    public static ApiError EdgesWithBuildingIdAndFloorNotFoundError(Guid buildingId, int floor)
    {
        return new ApiError(404, $"Ребер на этаже '{floor}' в здании с идетификатором '{buildingId}' не найдено.",
            GetUrn("edge-with-building-id-and-floor-not-found"));
    }
    
    public static ApiError EdgesWithBuildingObjectIdNotFoundError(Guid buildingObjectId)
    {
        return new ApiError(404, $"У объекта с идентификатором '{buildingObjectId}' нет ребер.",
            GetUrn("edge-with-building-object-id-not-found"));
    }
    
    public static ApiError FloorsWithBuildingIdNotFoundError(Guid buildingId)
    {
        return new ApiError(404, $"Этажи в здании с идентификатором '{buildingId}' не найдены.",
            GetUrn("floors-with-building-id-not-found"));
    }
    
    public static ApiError FloorWithBuildingAndFloorNumberIdNotFoundError(Guid buildingId, int floorNumber)
    {
        return new ApiError(404, $"Этаж '{floorNumber}' в здании с идентификатором '{buildingId}' не найден.",
            GetUrn("floor-with-building-id-and-floor-number-not-found"));
    }
    
    public static ApiError ImageWithIdNotFoundError(Guid imageId)
    {
        return new ApiError(404, $"Изображение с идентификатором '{imageId}' не найдено.",
            GetUrn("image-with-id-not-found"));
    }
    
    public static ApiError ImageWithFloorIdNotFoundError(Guid buildingId, int floorNumber)
    {
        return new ApiError(404,
            $"Изображения на этаже '{floorNumber}' в здании с идентификатором '{buildingId}' не найдено.",
            GetUrn("image-with-floor-id-not-found"));
    }
    

    public static ApiError EmptyListTypesError()
    {
        return new ApiError(400,
            "Невозможно найти объекты с пустым списком типов.",
            GetUrn("building-objects-with-id-and-type-not-found"));
    }
    
    public static ApiError BuildingAlreadyExistsError(Guid buildingId)
    {
        return new ApiError(400,
            $"Здание с идентификатором '{buildingId}' уже существует.",
            GetUrn("building-already-exists"));
    }
    
    public static ApiError TargetObjectIsEqualToSourceError(Guid objectId)
    {
        return new ApiError(400, $"Объект с идентификатором '{objectId}' является целевым и совпадает с исходным.",
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