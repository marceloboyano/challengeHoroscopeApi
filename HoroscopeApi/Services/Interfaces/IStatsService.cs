using HoroscopeApi.DTOs.Responses;

namespace HoroscopeApi.Services.Interfaces;

public interface IStatsService
{
    Task<ServiceResult<SignStatResponse>> GetMostSearchedSignAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult<IEnumerable<SignStatResponse>>> GetRankingAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult<PagedResponse<HistoryItemResponse>>> GetHistoryAsync(int userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
