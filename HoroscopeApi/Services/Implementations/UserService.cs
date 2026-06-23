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

    public async Task<ServiceResult<ProfileResponse>> GetProfileAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return ServiceResult<ProfileResponse>.Fail(Messages.User.NotFound, 404);
        }

        return ServiceResult<ProfileResponse>.Ok(user.ToProfileResponse());
    }

    public async Task<ServiceResult<ProfileResponse>> UpdateProfileAsync(int userId, UpdateProfileRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return ServiceResult<ProfileResponse>.Fail(Messages.User.NotFound, 404);
        }
      
        var hasChanges = false;

        if (!string.IsNullOrWhiteSpace(request.Email) &&
            !request.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase))
        {
            if (await _users.ExistsByEmailAsync(request.Email, cancellationToken))
            {
                return ServiceResult<ProfileResponse>.Fail(Messages.User.EmailTaken, 409);
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

        return ServiceResult<ProfileResponse>.Ok(user.ToProfileResponse(), Messages.User.ProfileUpdated);
    }
}
