namespace Istu.Navigation.Infrastructure.Errors;

public class CommonErrors 
{
    private static string GetUrn(string errorType) => $"urn:common-errors:{errorType}";
    
    public static ApiError InternalServerError()
    {
        return new ApiError(500, "Произошла внутренняя ошибка сервера.", GetUrn("internal-server-error"));
    }
    
    public static ApiError EmptyTitleError()
    {
        return new ApiError(400,
            "Свойство названия не может быть пустым.",
            GetUrn("title-is-empty"));
    }
}