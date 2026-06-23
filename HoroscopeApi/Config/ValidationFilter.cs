using FluentValidation;
using HoroscopeApi.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HoroscopeApi.Config;

/// <summary>
/// Filtro que ejecuta automaticamente los validators de FluentValidation
/// para los argumentos de accion y devuelve un 400 (ApiResponse) si hay errores.
/// </summary>
public class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null) continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());

            if (_serviceProvider.GetService(validatorType) is not IValidator validator) continue;

            var validationContext = new ValidationContext<object>(argument);
            var result = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);

            if (!result.IsValid)
            {
                var message = string.Join(" ", result.Errors.Select(e => e.ErrorMessage));
                context.Result = new BadRequestObjectResult(new ApiResponse<object>(message, StatusCodes.Status400BadRequest));
                return;
            }
        }

        await next();
    }
}
