using HoroscopeApi.DTOs.Responses;
using HoroscopeApi.Entities;
using HoroscopeApi.Helpers;

namespace HoroscopeApi.Mappings;

public static class EntityMapper
{
    public static ProfileResponseDto ToProfileResponse(this User user) => new()
    {
        Username = user.Username,
        Email = user.Email,
        BirthDate = user.BirthDate,
        Sign = ZodiacCalculator.GetSign(user.BirthDate)
    };

    public static HistoryItemResponseDto ToHistoryItemResponse(this HoroscopeQuery query) => new()
    {
        Sign = query.Sign,
        QueryDate = query.QueryDate,
        ResultText = query.ResultText,
        CreatedAt = query.CreatedAt
    };
}
