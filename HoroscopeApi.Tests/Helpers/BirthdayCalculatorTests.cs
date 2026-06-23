using HoroscopeApi.Helpers;

namespace HoroscopeApi.Tests.Helpers;

public class BirthdayCalculatorTests
{
    [Fact]
    public void DaysUntilNextBirthday_WhenBirthdayIsToday_ReturnsZero()
    {
        var today = new DateOnly(2026, 6, 22);
        var birthDate = new DateOnly(1990, 6, 22);

        var days = BirthdayCalculator.DaysUntilNextBirthday(birthDate, today);

        Assert.Equal(0, days);
    }

    [Fact]
    public void DaysUntilNextBirthday_WhenBirthdayIsTomorrow_ReturnsOne()
    {
        var today = new DateOnly(2026, 6, 22);
        var birthDate = new DateOnly(1990, 6, 23);

        var days = BirthdayCalculator.DaysUntilNextBirthday(birthDate, today);

        Assert.Equal(1, days);
    }

    [Fact]
    public void DaysUntilNextBirthday_WhenBirthdayAlreadyPassed_ReturnsDaysUntilNextYear()
    {
        var today = new DateOnly(2026, 6, 22);
        var birthDate = new DateOnly(1990, 6, 21);

        var days = BirthdayCalculator.DaysUntilNextBirthday(birthDate, today);

        // Proximo cumple: 2027-06-21
        var expected = new DateOnly(2027, 6, 21).DayNumber - today.DayNumber;
        Assert.Equal(expected, days);
        Assert.Equal(364, days);
    }

    [Fact]
    public void DaysUntilNextBirthday_Feb29_InNonLeapYear_UsesFeb28()
    {
        var today = new DateOnly(2025, 2, 1); // 2025 no es bisiesto
        var birthDate = new DateOnly(2000, 2, 29);

        var days = BirthdayCalculator.DaysUntilNextBirthday(birthDate, today);

        // Cumple ajustado a 2025-02-28
        Assert.Equal(27, days);
    }

    [Fact]
    public void DaysUntilNextBirthday_Feb29_InLeapYear_UsesFeb29()
    {
        var today = new DateOnly(2024, 2, 1); // 2024 es bisiesto
        var birthDate = new DateOnly(2000, 2, 29);

        var days = BirthdayCalculator.DaysUntilNextBirthday(birthDate, today);

        // Cumple 2024-02-29
        Assert.Equal(28, days);
    }
}
