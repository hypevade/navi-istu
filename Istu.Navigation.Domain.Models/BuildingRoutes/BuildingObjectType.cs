namespace Istu.Navigation.Domain.Models.BuildingRoutes;

public enum BuildingObjectType
{
    Node = 0,
    Auditorium = 1,
    Cabinet = 2,
    Toilet = 3,
    Cafe = 4,
    Ladder = 5,
    Elevator = 6,
    Entrance = 7,
    CashRegister = 8,
    Showplace = 9,
    Wardrobe = 10,
    SportArea = 11
}

public static class BuildingObjectTypeExtensions
{
    public static string GetRussianName(this BuildingObjectType type)
    {
        return type switch
        {
            BuildingObjectType.Node => "Узел",
            BuildingObjectType.Auditorium => "Аудитория",
            BuildingObjectType.Cabinet => "Кабинет",
            BuildingObjectType.Toilet => "Туалет",
            BuildingObjectType.Cafe => "Кафе",
            BuildingObjectType.Ladder => "Лестница",
            BuildingObjectType.Elevator => "Лифт",
            BuildingObjectType.Entrance => "Вход",
            BuildingObjectType.CashRegister => "Касса",
            BuildingObjectType.Showplace => "Достопримечательность",
            BuildingObjectType.Wardrobe => "Гардероб",
            BuildingObjectType.SportArea => "Спортивная зона",
            
            _ => "Неизвестно"
        };
    }
    
    public static bool IsPublicObject(this BuildingObjectType type)
    {
        return type == BuildingObjectType.Auditorium || type == BuildingObjectType.Cabinet ||
               type == BuildingObjectType.Cafe || type == BuildingObjectType.CashRegister ||
               type == BuildingObjectType.Showplace;
    }
    
}