using HoroscopeApi.DTOs.Requests;
using HoroscopeApi.DTOs.Responses;

namespace HoroscopeApi.Services.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
