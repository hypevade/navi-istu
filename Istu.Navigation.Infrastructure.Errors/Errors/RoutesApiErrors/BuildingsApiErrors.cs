﻿namespace Istu.Navigation.Infrastructure.Errors.Errors.RoutesApiErrors;

public static class BuildingsApiErrors
{
    private static string GetUrn(string errorType) => $"urn:buildings-api-errors:{errorType}";
    
    public static ApiError BuildingWithIdNotFoundError(Guid buildingId)
    {
        return new ApiError(404, $"Здание с идентификатором '{buildingId}' не найдено.",
            GetUrn("building-with-id-not-found"));
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
    
    public static ApiError ImageWithFloorIdNotFoundError(Guid buildingId, int floorNumber)
    {
        return new ApiError(404,
            $"Изображения на этаже '{floorNumber}' в здании с идентификатором '{buildingId}' не найдено.",
            GetUrn("image-with-floor-id-not-found"));
    }
    
    public static ApiError NoFloorsError()
    {
        return new ApiError(400,
            "Невозможно создать здание без этажей.",
            GetUrn("zero-floor-numbers"));
    }
    
    public static ApiError BuildingAlreadyExistsError(Guid buildingId)
    {
        return new ApiError(400,
            $"Здание с идентификатором '{buildingId}' уже существует.",
            GetUrn("building-already-exists"));
    }
    
    public static ApiError FloorContainsNoObjectsError(Guid buildingId, int floor)
    {
        return new ApiError(400,
            $"Этаж {floor} в здании с идентификатором {buildingId} не содержит объектов. Возможно вам стоит добавить их.",
            GetUrn("floor-contains-no-objects"));
    }
    
    public static ApiError WrongFloorNumberError(int floor, int expectedFloor)
    {
        return new ApiError(400,
            $"Передан этаж с неверным номером {floor}. Ожидаемый этаж {expectedFloor}.",
            GetUrn("wrong-floor-number"));
    }

}