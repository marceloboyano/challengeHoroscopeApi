namespace HoroscopeApi.DTOs.Responses;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T? Data { get; init; }
    public int StatusCode { get; set; }

    public ApiResponse(T data, string message = "Success", int statusCode = 200)
    {
        Success = true;
        Data = data;
        Message = message;
        StatusCode = statusCode;
    }

    public ApiResponse(string errorMessage, int statusCode)
    {
        Success = false;
        Message = errorMessage;
        StatusCode = statusCode;
    }
}
