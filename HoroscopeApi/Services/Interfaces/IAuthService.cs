using HoroscopeApi.DTOs.Requests;
using HoroscopeApi.DTOs.Responses;

namespace HoroscopeApi.Services.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);
    Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
}
