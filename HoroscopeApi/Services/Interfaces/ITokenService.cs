using HoroscopeApi.Entities;

namespace HoroscopeApi.Services.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}
