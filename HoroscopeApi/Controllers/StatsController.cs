using HoroscopeApi.DTOs.Responses;
using HoroscopeApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoroscopeApi.Controllers;

[Authorize]
[Route("api/[controller]")]
public class StatsController : ApiControllerBase
{
    private readonly IStatsService _statsService;

    public StatsController(IStatsService statsService)
    {
        _statsService = statsService;
    }

    /// <summary>Returns the most searched sign across the whole system.</summary>
    [HttpGet("most-searched")]
    [ProducesResponseType(typeof(ApiResponse<SignStatResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MostSearched(CancellationToken cancellationToken)
    {
        var result = await _statsService.GetMostSearchedSignAsync(cancellationToken);
        return FromResult(result);
    }

    /// <summary>Returns the sign ranking by number of queries.</summary>
    [HttpGet("ranking")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SignStatResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Ranking(CancellationToken cancellationToken)
    {
        var result = await _statsService.GetRankingAsync(cancellationToken);
        return FromResult(result);
    }

    /// <summary>Returns the authenticated user's query history (paginated).</summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<HistoryItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> History([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _statsService.GetHistoryAsync(CurrentUserId, pageNumber, pageSize, cancellationToken);
        return FromResult(result);
    }
}
