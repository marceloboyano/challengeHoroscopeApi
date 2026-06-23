using HoroscopeApi.DTOs.Responses;
using HoroscopeApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoroscopeApi.Controllers;

[Authorize]
[Route("api/[controller]")]
public class HoroscopeController : ApiControllerBase
{
    private readonly IHoroscopeService _horoscopeService;

    public HoroscopeController(IHoroscopeService horoscopeService)
    {
        _horoscopeService = horoscopeService;
    }

    /// <summary>
    /// Devuelve el horoscope del dia para el usuario autenticado, su signo
    /// y los dias que faltan para su proximo cumpleanos.
    /// </summary>
    [HttpGet("today")]
    [ProducesResponseType(typeof(ApiResponse<HoroscopeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetToday(CancellationToken cancellationToken)
    {
        var result = await _horoscopeService.GetTodayAsync(CurrentUserId, cancellationToken);
        return FromResult(result);
    }
}
