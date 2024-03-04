namespace Istu.Navigation.Infrastructure.Errors;

public class OperationResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public ApiError? ErrorMessage { get; set; }

    // Фабричные методы для создания результатов
    public static OperationResult<T> Success(T data) => new() { IsSuccess = true, Data = data };
    public static OperationResult<T> Failure(ApiError apiError) => new() { IsSuccess = false, ErrorMessage = apiError };
}
