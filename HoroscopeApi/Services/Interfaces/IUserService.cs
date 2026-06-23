using HoroscopeApi.DTOs.Requests;
using HoroscopeApi.DTOs.Responses;

namespace HoroscopeApi.Services.Interfaces;

public interface IUserService
{
    Task<ServiceResult<ProfileResponseDto>> GetProfileAsync(int userId, CancellationToken cancellationToken = default);
    Task<ServiceResult<ProfileResponseDto>> UpdateProfileAsync(int userId, UpdateProfileRequestDto request, CancellationToken cancellationToken = default);
}
