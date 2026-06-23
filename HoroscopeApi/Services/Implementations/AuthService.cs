using HoroscopeApi.Constants;
using HoroscopeApi.DTOs.Requests;
using HoroscopeApi.DTOs.Responses;
using HoroscopeApi.Entities;
using HoroscopeApi.Repositories.Interfaces;
using HoroscopeApi.Services.Interfaces;

namespace HoroscopeApi.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository users, ITokenService tokenService)
    {
        _users = users;
        _tokenService = tokenService;
    }

    public async Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (await _users.ExistsByUsernameAsync(request.Username, cancellationToken))
        {
            return ServiceResult<AuthResponse>.Fail(Messages.Auth.UsernameTaken, 409);
        }

        if (await _users.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            return ServiceResult<AuthResponse>.Fail(Messages.User.EmailTaken, 409);
        }

        var user = new User
        {
            Username = request.Username.Trim(),
            Email = request.Email.Trim(),
            BirthDate = request.BirthDate,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await _users.AddAsync(user, cancellationToken);

        var auth = BuildAuthResponse(user);
        return ServiceResult<AuthResponse>.Ok(auth, Messages.Auth.Registered, 201);
    }

    public async Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByUsernameAsync(request.Username, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return ServiceResult<AuthResponse>.Fail(Messages.Auth.InvalidCredentials, 401);
        }

        var auth = BuildAuthResponse(user);
        return ServiceResult<AuthResponse>.Ok(auth, Messages.Auth.LoginSuccess);
    }

    private AuthResponse BuildAuthResponse(User user)
    {
        var (token, expiresAt) = _tokenService.GenerateToken(user);
        return new AuthResponse
        {
            Token = token,
            Username = user.Username,
            ExpiresAt = expiresAt
        };
    }
}
