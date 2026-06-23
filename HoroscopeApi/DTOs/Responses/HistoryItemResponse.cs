namespace HoroscopeApi.DTOs.Responses;

public class HistoryItemResponse
{
    public string Sign { get; set; } = null!;
    public DateOnly QueryDate { get; set; }
    public string ResultText { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
