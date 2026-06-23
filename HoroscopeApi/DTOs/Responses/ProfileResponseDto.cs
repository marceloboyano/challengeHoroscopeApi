namespace HoroscopeApi.DTOs.Responses;

public class ProfileResponseDto
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateOnly BirthDate { get; set; }
    public string Sign { get; set; } = null!;
}
