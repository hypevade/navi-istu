namespace Istu.Navigation.Api.Paths;

public static class ApiRoutes
{
    public const string Root = "api";
    public const string Version = "v1";
    public const string Base = Root + "/" + Version;
    
    public static class Buildings
    {
        public const string BuildingsApi = Base + "/buildings";
        public const string GetAllPart = "";
        public const string CreatePart = "";
        public const string UpdatePart = "";
        
        public const string GetPart = "{buildingId}";
        public const string DeletePart = "{buildingId}";

        public static string GetWithIdRoute(Guid buildingId)
        {
            return BuildingsApi + "/" + GetPart.Replace("{buildingId}", buildingId.ToString());
        }
        
        public static string DeleteRoute(Guid buildingId)
        {
            return BuildingsApi + "/" + DeletePart.Replace("{buildingId}", buildingId.ToString());
        }
        
        public static string UpdateRoute()
        {
            return BuildingsApi + "/" + UpdatePart;
        }
        
        public static string GetAllRoute()
        {
            return BuildingsApi + "/" + GetAllPart;
        }
        
        public static string CreateRoute()
        {
            return BuildingsApi + "/" + CreatePart;
        }
    }

    public static class BuildingRoutes
    {
        public const string BuildingsRoutesApi = Base + "/buildings/routes";
        public const string CreatePart = "";
        
        public static string CreateRoute()
        {
            return BuildingsRoutesApi + "/" + CreatePart;
        }
    }

    public static class BuildingObjects
    {
        public const string BuildingsObjectsApi = Base + "/buildings/objects";
        public const string CreatePart = "";
        public const string GetAllPart = "";
        public const string GetByIdPart = "{objectId}";
        public const string GetEdgesByIdPart = "{objectId}/edges";
        public const string CreateEdgesPart = "edges";
        
        public static string CreateObjectRoute()
        {
            return BuildingsObjectsApi + "/" + CreatePart;
        }
        
        public static string GetAllRoute()
        {
            return BuildingsObjectsApi + "/" + GetAllPart;
        }
        
        public static string GetWithIdRoute(Guid buildingObjectId)
        {
            return BuildingsObjectsApi + "/" + GetByIdPart.Replace("{objectId}", buildingObjectId.ToString());
        }
        
        public static string GetEdgesByObjectIdRoute(Guid buildingObjectId)
        {
            return BuildingsObjectsApi + "/" + GetEdgesByIdPart.Replace("{objectId}", buildingObjectId.ToString());
        }
        
        public static string CreateEdgesRoute()
        {
            return BuildingsObjectsApi + "/" + CreateEdgesPart;
        }
    }
}