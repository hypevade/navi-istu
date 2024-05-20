namespace Istu.Navigation.Infrastructure.Errors.UsersApiErrors;

public class UsersApiErrors
{
    private static string GetUrn(string errorType) => $"urn:users-api-errors:{errorType}";
    
    public static ApiError UserWithEmailNotFoundError(string email)
    {
        return new ApiError(404, $"Пользователь с почтой '{email}' не найден.",
            GetUrn("user-with-email-not-found"));
    }
    
    public static ApiError IncorrectPasswordError(string email)
    {
        return new ApiError(401, $"Неверный пароль для пользователя с почтой '{email}'.",
            GetUrn("incorrect-password"));
    }
    public static ApiError TokenExpiredError()
    {
        return new ApiError(401, $"Токен устарел.",
            GetUrn("token-expired"));
    }
    public static ApiError TokenIsNotValidError()
    {
        return new ApiError(401, $"Токен недействителен.",
            GetUrn("token-is-not-valid"));
    }
    
    public static ApiError AuthorizationHeaderIsEmptyError()
    {
        return new ApiError(401, "Заголовок 'Authorization' не может быть пустым.",
            GetUrn("authorization-header-is-empty"));
    }
    public static ApiError CodeNotValidError()
    {
        return new ApiError(401, $"Код недействителен.",
            GetUrn("code-not-valid"));
    }

    public static ApiError AccessDeniedError()
    {
        return new ApiError(403, $"Доступ запрещен.",GetUrn("access-denied"));
    }
}