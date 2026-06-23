using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HoroscopeApi.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;

namespace HoroscopeApi.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Obtiene el Id del usuario autenticado desde el claim 'sub' del token JWT.
    /// </summary>
    protected int CurrentUserId
    {
        get
        {
            var value = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                        ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(value, out var id) ? id : 0;
        }
    }

    /// <summary>
    /// Convierte un ServiceResult en la respuesta HTTP estandar (ApiResponse).
    /// </summary>
    protected IActionResult FromResult<T>(ServiceResult<T> result)
    {
        if (result.Success)
        {
            return StatusCode(result.StatusCode, new ApiResponse<T>(result.Data!, result.Message, result.StatusCode));
        }

        return StatusCode(result.StatusCode, new ApiResponse<object>(result.Message, result.StatusCode));
    }
}
