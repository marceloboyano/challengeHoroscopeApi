using HoroscopeApi.Constants;
using HoroscopeApi.DTOs.Requests;
using HoroscopeApi.DTOs.Responses;
using HoroscopeApi.Mappings;
using HoroscopeApi.Repositories.Interfaces;
using HoroscopeApi.Services.Interfaces;

namespace HoroscopeApi.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _users;

    public UserService(IUserRepository users)
    {
        _users = users;
    }

    public async Task<ServiceResult<ProfileResponseDto>> GetProfileAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return ServiceResult<ProfileResponseDto>.Fail(Messages.User.NotFound, 404);
        }

        return ServiceResult<ProfileResponseDto>.Ok(user.ToProfileResponse());
    }

    public async Task<ServiceResult<ProfileResponseDto>> UpdateProfileAsync(int userId, UpdateProfileRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return ServiceResult<ProfileResponseDto>.Fail(Messages.User.NotFound, 404);
        }
      
        var hasChanges = false;

        if (!string.IsNullOrWhiteSpace(request.Email) &&
            !request.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase))
        {
            if (await _users.ExistsByEmailAsync(request.Email, cancellationToken))
            {
                return ServiceResult<ProfileResponseDto>.Fail(Messages.User.EmailTaken, 409);
            }
            user.Email = request.Email.Trim();
            hasChanges = true;
        }

        if (request.BirthDate.HasValue && request.BirthDate.Value != user.BirthDate)
        {
            user.BirthDate = request.BirthDate.Value;
            hasChanges = true;
        }

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            hasChanges = true;
        }

        if (hasChanges)
        {
            await _users.UpdateAsync(user, cancellationToken);
        }

        return ServiceResult<ProfileResponseDto>.Ok(user.ToProfileResponse(), Messages.User.ProfileUpdated);
    }
}
