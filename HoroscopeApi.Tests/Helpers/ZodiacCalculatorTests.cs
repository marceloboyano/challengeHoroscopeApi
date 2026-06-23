using HoroscopeApi.Helpers;

namespace HoroscopeApi.Tests.Helpers;

public class ZodiacCalculatorTests
{
    [Theory]
    // Bordes de cada signo (ultimo dia del signo y primer dia del siguiente)
    [InlineData(1, 19, "Capricorn")]
    [InlineData(1, 20, "Aquarius")]
    [InlineData(2, 18, "Aquarius")]
    [InlineData(2, 19, "Pisces")]
    [InlineData(3, 20, "Pisces")]
    [InlineData(3, 21, "Aries")]
    [InlineData(4, 19, "Aries")]
    [InlineData(4, 20, "Taurus")]
    [InlineData(5, 20, "Taurus")]
    [InlineData(5, 21, "Gemini")]
    [InlineData(6, 20, "Gemini")]
    [InlineData(6, 21, "Cancer")]
    [InlineData(7, 22, "Cancer")]
    [InlineData(7, 23, "Leo")]
    [InlineData(8, 22, "Leo")]
    [InlineData(8, 23, "Virgo")]
    [InlineData(9, 22, "Virgo")]
    [InlineData(9, 23, "Libra")]
    [InlineData(10, 22, "Libra")]
    [InlineData(10, 23, "Scorpio")]
    [InlineData(11, 21, "Scorpio")]
    [InlineData(11, 22, "Sagittarius")]
    [InlineData(12, 21, "Sagittarius")]
    [InlineData(12, 22, "Capricorn")]
    public void GetSign_ReturnsExpectedSign_AtBoundaries(int month, int day, string expected)
    {
        var birthDate = new DateOnly(1990, month, day);

        var sign = ZodiacCalculator.GetSign(birthDate);

        Assert.Equal(expected, sign);
    }

    [Fact]
    public void GetSign_MidRangeDate_ReturnsLibra()
    {
        var sign = ZodiacCalculator.GetSign(new DateOnly(1990, 10, 5));

        Assert.Equal("Libra", sign);
    }
}
