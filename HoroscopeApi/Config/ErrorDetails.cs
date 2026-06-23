using System.Text.Json;

namespace HoroscopeApi.Config;

public class ErrorDetails
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = null!;
    public string? Detailed { get; set; }
    public string? StackTrace { get; set; }

    public override string ToString() => JsonSerializer.Serialize(this);
}
