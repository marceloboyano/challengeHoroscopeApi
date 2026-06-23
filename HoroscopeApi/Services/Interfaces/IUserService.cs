using HoroscopeApi.DTOs.Requests;
using HoroscopeApi.DTOs.Responses;

namespace HoroscopeApi.Services.Interfaces;

public interface IUserService
{
    Task<ServiceResult<ProfileResponse>> GetProfileAsync(int userId, CancellationToken cancellationToken = default);
    Task<ServiceResult<ProfileResponse>> UpdateProfileAsync(int userId, UpdateProfileRequest request, CancellationToken cancellationToken = default);
}
