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

    public async Task<ServiceResult<SignStatResponse>> GetMostSearchedSignAsync(CancellationToken cancellationToken = default)
    {
        var stats = await _queries.GetSignStatsAsync(cancellationToken);
        var top = stats.FirstOrDefault();

        if (top.Sign is null)
        {
            return ServiceResult<SignStatResponse>.Fail(Messages.Stats.NoQueries, 404);
        }

        return ServiceResult<SignStatResponse>.Ok(new SignStatResponse
        {
            Sign = top.Sign,
            Count = top.Count
        });
    }

    public async Task<ServiceResult<IEnumerable<SignStatResponse>>> GetRankingAsync(CancellationToken cancellationToken = default)
    {
        var stats = await _queries.GetSignStatsAsync(cancellationToken);

        var ranking = stats.Select(s => new SignStatResponse
        {
            Sign = s.Sign,
            Count = s.Count
        });

        return ServiceResult<IEnumerable<SignStatResponse>>.Ok(ranking);
    }

    public async Task<ServiceResult<PagedResponse<HistoryItemResponse>>> GetHistoryAsync(int userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 50) pageSize = 10;

        var (items, totalCount) = await _queries.GetByUserPagedAsync(userId, pageNumber, pageSize, cancellationToken);

        var mapped = items.Select(q => q.ToHistoryItemResponse());

        var paged = new PagedResponse<HistoryItemResponse>(mapped, totalCount, pageNumber, pageSize);

        return ServiceResult<PagedResponse<HistoryItemResponse>>.Ok(paged);
    }
}
