using System.Net;
using HoroscopeApi.Constants;

namespace HoroscopeApi.Config;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new ErrorDetails
        {
            StatusCode = context.Response.StatusCode,
            Message = Messages.General.InternalError,
            Detailed = _env.IsDevelopment() ? exception.Message : null,
            StackTrace = _env.IsDevelopment() ? exception.StackTrace : null
        };

        await context.Response.WriteAsync(response.ToString());
    }
}
