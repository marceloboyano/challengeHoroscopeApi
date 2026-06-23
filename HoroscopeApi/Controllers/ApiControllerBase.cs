using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HoroscopeApi.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;

namespace HoroscopeApi.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected int CurrentUserId
    {
        get
        {
            var value = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                        ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(value, out var id) ? id : 0;
        }
    }

    protected IActionResult FromResult<T>(ServiceResult<T> result)
    {
        if (result.Success)
        {
            return StatusCode(result.StatusCode, new ApiResponse<T>(result.Data!, result.Message, result.StatusCode));
        }

        return StatusCode(result.StatusCode, new ApiResponse<object>(result.Message, result.StatusCode));
    }
}
