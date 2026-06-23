using System.Text;
using System.Text.Json;
using HoroscopeApi.Constants;
using HoroscopeApi.DTOs.External;
using HoroscopeApi.Services.Interfaces;

namespace HoroscopeApi.Services.Implementations;

public class NewAstroClient : INewAstroClient
{
    private readonly HttpClient _http;
    private readonly ILogger<NewAstroClient> _logger;

    public NewAstroClient(HttpClient http, ILogger<NewAstroClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<string> GetHoroscopeAsync(string sign, DateOnly date, string lang, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            date = date.ToString("yyyy-MM-dd"),
            lang,
            sign
        };

        var json = JsonSerializer.Serialize(payload);
        // Usamos StringContent (con Content-Length explicito) en vez de PostAsJsonAsync,
        // porque el edge de Vercel rechaza los POST con transfer-encoding chunked.
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _http.PostAsync("/", content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("API horoscope respondio {Status}. Body: {Body}", (int)response.StatusCode, errorBody);
            response.EnsureSuccessStatusCode();
        }

        var result = await response.Content.ReadFromJsonAsync<NewAstroResponse>(cancellationToken);

        if (result is null || string.IsNullOrWhiteSpace(result.Horoscope))
        {
            _logger.LogWarning("La API de horoscope no devolvio contenido para el signo {Sign}", sign);
            return Messages.Horoscope.Unavailable;
        }

        return result.Horoscope;
    }
}
