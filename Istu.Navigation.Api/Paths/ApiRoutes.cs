namespace Istu.Navigation.Api.Paths;

public static class ApiRoutes
{
    public const string Root = "api";
    public const string Version = "v1";
    public const string Base = Root + "/" + Version;
    
    public static class Buildings
    {
        public const string BuildingsApi = Base + "/buildings";
        public const string GetAllBuildingsPart = "";
        public const string CreateBuildingPart = "";
        public const string UpdateBuildingPart = "";
        public const string GetBuildingPart = "{buildingId}";
        public const string DeleteBuildingPart = "{buildingId}";
        
        public const string CreateFloorPart = "{buildingId}/floors";
        public const string GetFloorPart = "{buildingId}/floors/{floorNumber}";
        public const string DeleteFloorPart = "{buildingId}/floors/{floorNumber}";
        public const string GetFloorsPart = "{buildingId}/floors";
        

        public static string GetBuildingWithIdRoute(Guid buildingId)
        {
            return BuildingsApi + "/" + GetBuildingPart.Replace("{buildingId}", buildingId.ToString());
        }
        
        public static string DeleteBuildingRoute(Guid buildingId)
        {
            return BuildingsApi + "/" + DeleteBuildingPart.Replace("{buildingId}", buildingId.ToString());
        }
        
        public static string UpdateBuildingRoute()
        {
            return BuildingsApi + UpdateBuildingPart;
        }
        
        public static string GetAllBuildingsRoute()
        {
            return BuildingsApi + GetAllBuildingsPart;
        }
        
        public static string CreateBuildingRoute()
        {
            return BuildingsApi + CreateBuildingPart;
        }
        
        public static string CreateFloorRoute(Guid buildingId)
        {
            return BuildingsApi + "/" + CreateFloorPart.Replace("{buildingId}", buildingId.ToString());
        }

        public static string DeleteFloorRoute(Guid buildingId, int floorNumber)
        {
            return BuildingsApi + "/" + DeleteFloorPart.Replace("{buildingId}", buildingId.ToString())
                .Replace("{floorNumber}", floorNumber.ToString());
        }
        
        public static string GetFloorRoute(Guid buildingId, int floorNumber)
        {
            return BuildingsApi + "/" + GetFloorPart.Replace("{buildingId}", buildingId.ToString())
                .Replace("{floorNumber}", floorNumber.ToString());
        }
    }

    public static class BuildingRoutes
    {
        public const string BuildingsRoutesApi = Base + "/buildings/routes";
        public const string CreatePart = "";
        
        public static string CreateRoute()
        {
            return BuildingsRoutesApi + CreatePart;
        }
    }
    
    public static class BuildingEdges
    {
        public const string EdgesApi = Base + "/buildings/edges";
        public const string CreatePart = "";
        public const string DeletePart = "{edgeId}";
        public const string GetAllPart = "";
        
        public static string CreateRoute()
        {
            return EdgesApi + CreatePart;
        }
        
        public static string GetAllRoute()
        {
            return EdgesApi + GetAllPart;
        }
        public static string GetDeleteRoute(Guid edgeId)
        {
            return EdgesApi + "/" + DeletePart.Replace("{edgeId}", edgeId.ToString());
        }
    }

    public static class BuildingObjects
    {
        public const string BuildingsObjectsApi = Base + "/buildings/objects";
        public const string CreatePart = "";
        public const string UpdatePart = "";
        public const string DeletePart = "{objectId}";
        public const string GetAllPart = "";
        public const string GetByIdPart = "{objectId}";
        
        public static string CreateRoute()
        {
            return BuildingsObjectsApi + CreatePart;
        }
        
        public static string UpdateObjectRoute()
        {
            return BuildingsObjectsApi + UpdatePart;
        }
        
        public static string DeleteObjectRoute(Guid buildingObjectId)
        {
            return BuildingsObjectsApi + "/" + GetByIdPart.Replace("{objectId}", buildingObjectId.ToString());
        }
        
        public static string GetAllRoute()
        {
            return BuildingsObjectsApi + GetAllPart;
        }
        
        public static string GetWithIdRoute(Guid buildingObjectId)
        {
            return BuildingsObjectsApi + "/" + GetByIdPart.Replace("{objectId}", buildingObjectId.ToString());
        }
    }

    public static class Users
    {
        public const string UsersApi = Base + "/users";
        public const string LoginPart = "login";
        public const string RegisterPart = "register";
    }
}