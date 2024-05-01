﻿namespace Istu.Navigation.Infrastructure.Errors.Errors.UsersApiErrors;

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
}