namespace HoroscopeApi.Helpers;

public static class BirthdayCalculator
{
    /// <summary>
    /// Calcula la cantidad de dias que faltan para el proximo cumpleanos.
    /// Si el cumpleanos es hoy, devuelve 0.
    /// Maneja el caso 29/02 usando el 28/02 en anios no bisiestos.
    /// </summary>
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
