namespace HoroscopeApi.DTOs.Responses;

public class AuthResponse
{
    public string Token { get; set; } = null!;
    public string Username { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}
