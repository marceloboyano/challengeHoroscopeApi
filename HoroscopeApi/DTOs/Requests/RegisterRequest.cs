namespace HoroscopeApi.DTOs.Requests;

public class RegisterRequest
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateOnly BirthDate { get; set; }
    public string Password { get; set; } = null!;
}
