using HoroscopeApi.DTOs.Responses;

namespace HoroscopeApi.Services.Interfaces;

public interface IStatsService
{
    Task<ServiceResult<SignStatResponseDto>> GetMostSearchedSignAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult<IEnumerable<SignStatResponseDto>>> GetRankingAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult<PagedResponse<HistoryItemResponseDto>>> GetHistoryAsync(int userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
