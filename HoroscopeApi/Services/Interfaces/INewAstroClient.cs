namespace HoroscopeApi.Services.Interfaces;

public interface INewAstroClient
{
    Task<string> GetHoroscopeAsync(string sign, DateOnly date, string lang, CancellationToken cancellationToken = default);
}
