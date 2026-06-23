using HoroscopeApi.Constants;
using HoroscopeApi.DTOs.Responses;
using HoroscopeApi.Mappings;
using HoroscopeApi.Repositories.Interfaces;
using HoroscopeApi.Services.Interfaces;

namespace HoroscopeApi.Services.Implementations;

public class StatsService : IStatsService
{
    private readonly IHoroscopeQueryRepository _queries;

    public StatsService(IHoroscopeQueryRepository queries)
    {
        _queries = queries;
    }

    public async Task<ServiceResult<SignStatResponseDto>> GetMostSearchedSignAsync(CancellationToken cancellationToken = default)
    {
        var stats = await _queries.GetSignStatsAsync(cancellationToken);
        var top = stats.FirstOrDefault();

        if (top.Sign is null)
        {
            return ServiceResult<SignStatResponseDto>.Fail(Messages.Stats.NoQueries, 404);
        }

        return ServiceResult<SignStatResponseDto>.Ok(new SignStatResponseDto
        {
            Sign = top.Sign,
            Count = top.Count
        });
    }

    public async Task<ServiceResult<IEnumerable<SignStatResponseDto>>> GetRankingAsync(CancellationToken cancellationToken = default)
    {
        var stats = await _queries.GetSignStatsAsync(cancellationToken);

        var ranking = stats.Select(s => new SignStatResponseDto
        {
            Sign = s.Sign,
            Count = s.Count
        });

        return ServiceResult<IEnumerable<SignStatResponseDto>>.Ok(ranking);
    }

    public async Task<ServiceResult<PagedResponse<HistoryItemResponseDto>>> GetHistoryAsync(int userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 50) pageSize = 10;

        var (items, totalCount) = await _queries.GetByUserPagedAsync(userId, pageNumber, pageSize, cancellationToken);

        var mapped = items.Select(q => q.ToHistoryItemResponse());

        var paged = new PagedResponse<HistoryItemResponseDto>(mapped, totalCount, pageNumber, pageSize);

        return ServiceResult<PagedResponse<HistoryItemResponseDto>>.Ok(paged);
    }
}
