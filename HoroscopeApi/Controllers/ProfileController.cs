using HoroscopeApi.DTOs.Requests;
using HoroscopeApi.DTOs.Responses;
using HoroscopeApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoroscopeApi.Controllers;

[Authorize]
[Route("api/[controller]")]
public class ProfileController : ApiControllerBase
{
    private readonly IUserService _userService;

    public ProfileController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>Returns the authenticated user's profile.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<ProfileResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _userService.GetProfileAsync(CurrentUserId, cancellationToken);
        return FromResult(result);
    }

    /// <summary>Updates the authenticated user's profile (username cannot be changed).</summary>
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<ProfileResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateProfileAsync(CurrentUserId, request, cancellationToken);
        return FromResult(result);
    }
}
