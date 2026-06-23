namespace HoroscopeApi.DTOs.Responses;

public class ServiceResult<T>
{
    public bool Success { get; private set; }
    public string Message { get; private set; }
    public int StatusCode { get; private set; }
    public T? Data { get; private set; }

    private ServiceResult(bool success, string message, int statusCode, T? data)
    {
        Success = success;
        Message = message;
        StatusCode = statusCode;
        Data = data;
    }

    public static ServiceResult<T> Ok(T data, string message = "Success", int statusCode = 200)
        => new(true, message, statusCode, data);

    public static ServiceResult<T> Fail(string message, int statusCode = 400)
        => new(false, message, statusCode, default);
}
