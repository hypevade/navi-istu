using System.Diagnostics.CodeAnalysis;

namespace Istu.Navigation.Infrastructure.Errors;

public class OperationResult<T>
{
    public bool IsSuccess { get; private init; }
    public bool IsFailure => !IsSuccess;
    
    [AllowNull]
    public T Data { get; private init; }
    [AllowNull]
    public ApiError ApiError { get; private init; }

    // Фабричные методы для создания результатов
    public static OperationResult<T> Success(T data) => new() { IsSuccess = true, Data = data };
    public static OperationResult<T> Failure(ApiError apiError) => new() { IsSuccess = false, ApiError = apiError };
}

public class OperationResult
{
    public bool IsSuccess { get; private init; }
    public bool IsFailure => !IsSuccess;
    
    [AllowNull]
    public ApiError ApiError { get; private init; }

    // Фабричные методы для создания результатов
    public static OperationResult Success() => new() { IsSuccess = true };
    public static OperationResult Failure(ApiError apiError) => new() { IsSuccess = false, ApiError = apiError };
}
