namespace HolidaysPB.Application.Features.Holidays.Calculation.Utils;

internal static class HolidayDateHelper {
    internal static DateOnly NextMonday(DateOnly date) {
        int nextMondayDays = ((int)DayOfWeek.Monday - (int)date.DayOfWeek + 7) % 7;
        return date.AddDays(nextMondayDays);
    }
    internal static DateOnly GetEasterSunday(int year)
        => GetHolyWeekBeginning(year).AddDays(7);
        
    private static DateOnly GetHolyWeekBeginning(int year) {
        int a = year % 19;
        int b = year % 4;
        int c = year % 7;
        int d = (19 * a + 24) % 30;

        int days = d + (2 * b + 4 * c + 6 * d + 5) % 7;

        int day = 15 + days;
        int month = 3;
        if (day > 31) {
            day -= 31;
            month = 4;
        }

        return new DateOnly(year, month, day);
    }
}