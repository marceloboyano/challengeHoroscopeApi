namespace HoroscopeApi.DTOs.Responses;

public class HoroscopeResponseDto
{
    public string Sign { get; set; } = null!;
    public DateOnly Date { get; set; }
    public string Horoscope { get; set; } = null!;
    public int DaysUntilBirthday { get; set; }
}
