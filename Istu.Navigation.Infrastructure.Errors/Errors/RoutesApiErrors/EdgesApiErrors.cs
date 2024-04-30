namespace Istu.Navigation.Infrastructure.Errors.Errors.RoutesApiErrors;

public class EdgesApiErrors
{
    private static string GetUrn(string errorType) => $"urn:edges-api-errors:{errorType}";

    public static ApiError EdgeFromDifferentBuildingsError(Guid fromId, Guid toId)
    {
        return new ApiError(400,
            $"Объекты с идентификаторами '{fromId}' и '{toId}' должны быть в одном здании.",
            GetUrn("building-objects-with-different-building-id"));
    }
    public static ApiError EdgeFromToSameError(Guid fromId, Guid toId)
    {
        return new ApiError(400,
            $"Переданные идентификаторы '{fromId}' и '{toId}' совпадают.",
            GetUrn("building-objects-with-same-id"));
    }
    
    public static ApiError EdgeAlreadyExistsError(Guid fromId, Guid toId)
    {
        return new ApiError(400,
            $"Ребро с идентификаторами '{fromId}' и '{toId}' уже существует.",
            GetUrn("edge-already-exists"));
    }
}