using HoroscopeApi.DTOs.Responses;

namespace HoroscopeApi.Services.Interfaces;

public interface IHoroscopeService
{
    Task<ServiceResult<HoroscopeResponse>> GetTodayAsync(int userId, CancellationToken cancellationToken = default);
}
