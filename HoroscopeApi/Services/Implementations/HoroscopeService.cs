using HoroscopeApi.Constants;
using HoroscopeApi.DTOs.Responses;
using HoroscopeApi.Entities;
using HoroscopeApi.Helpers;
using HoroscopeApi.Repositories.Interfaces;
using HoroscopeApi.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace HoroscopeApi.Services.Implementations;

public class HoroscopeService : IHoroscopeService
{
    private const string Lang = "es";

    private readonly IUserRepository _users;
    private readonly IHoroscopeQueryRepository _queries;
    private readonly INewAstroClient _client;
    private readonly IMemoryCache _cache;

    public HoroscopeService(
        IUserRepository users,
        IHoroscopeQueryRepository queries,
        INewAstroClient client,
        IMemoryCache cache)
    {
        _users = users;
        _queries = queries;
        _client = client;
        _cache = cache;
    }

    public async Task<ServiceResult<HoroscopeResponse>> GetTodayAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return ServiceResult<HoroscopeResponse>.Fail(Messages.User.NotFound, 404);
        }

        var today = DateOnly.FromDateTime(DateTime.Now);
        var sign = ZodiacCalculator.GetSign(user.BirthDate);
        var daysUntilBirthday = BirthdayCalculator.DaysUntilNextBirthday(user.BirthDate, today);

        var horoscopeText = await GetHoroscopeTextAsync(sign, today, cancellationToken);

        await _queries.AddAsync(new HoroscopeQuery
        {
            UserId = user.Id,
            Sign = sign,
            QueryDate = today,
            ResultText = horoscopeText
        }, cancellationToken);

        var response = new HoroscopeResponse
        {
            Sign = sign,
            Date = today,
            Horoscope = horoscopeText,
            DaysUntilBirthday = daysUntilBirthday
        };

        return ServiceResult<HoroscopeResponse>.Ok(response);
    }

    private async Task<string> GetHoroscopeTextAsync(string sign, DateOnly date, CancellationToken cancellationToken)
    {
        var cacheKey = $"horoscope_{sign}_{date:yyyyMMdd}_{Lang}";

        if (_cache.TryGetValue(cacheKey, out string? cached) && cached is not null)
        {
            return cached;
        }

        var text = await _client.GetHoroscopeAsync(sign, date, Lang, cancellationToken);

        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = date.ToDateTime(TimeOnly.MaxValue),
            SlidingExpiration = TimeSpan.FromHours(6)
        };
        _cache.Set(cacheKey, text, options);

        return text;
    }
}
