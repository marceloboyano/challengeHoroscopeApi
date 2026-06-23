using System.Text.Json.Serialization;

namespace HoroscopeApi.DTOs.External;

public class NewAstroResponseDto
{
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    [JsonPropertyName("horoscope")]
    public string? Horoscope { get; set; }

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("sign")]
    public string? Sign { get; set; }
}
