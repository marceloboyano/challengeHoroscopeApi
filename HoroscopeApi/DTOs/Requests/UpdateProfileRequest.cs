namespace HoroscopeApi.DTOs.Requests;

public class UpdateProfileRequest
{
    public string? Email { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? Password { get; set; }
}
