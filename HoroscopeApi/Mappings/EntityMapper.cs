using HoroscopeApi.DTOs.Responses;
using HoroscopeApi.Entities;
using HoroscopeApi.Helpers;

namespace HoroscopeApi.Mappings;

/// <summary>
/// Mapeo manual entre entidades y DTOs (sin dependencias externas).
/// </summary>
public static class EntityMapper
{
    public static ProfileResponse ToProfileResponse(this User user) => new()
    {
        Username = user.Username,
        Email = user.Email,
        BirthDate = user.BirthDate,
        Sign = ZodiacCalculator.GetSign(user.BirthDate)
    };

    public static HistoryItemResponse ToHistoryItemResponse(this HoroscopeQuery query) => new()
    {
        Sign = query.Sign,
        QueryDate = query.QueryDate,
        ResultText = query.ResultText,
        CreatedAt = query.CreatedAt
    };
}
