namespace HoroscopeApi.Helpers;

public static class BirthdayCalculator
{
    public static int DaysUntilNextBirthday(DateOnly birthDate, DateOnly today)
    {
        DateOnly next = BuildBirthdayForYear(birthDate, today.Year);

        if (next < today)
        {
            next = BuildBirthdayForYear(birthDate, today.Year + 1);
        }

        return next.DayNumber - today.DayNumber;
    }

    private static DateOnly BuildBirthdayForYear(DateOnly birthDate, int year)
    {
        int day = birthDate.Day;
        int month = birthDate.Month;

        if (month == 2 && day == 29 && !DateTime.IsLeapYear(year))
        {
            day = 28;
        }

        return new DateOnly(year, month, day);
    }
}
