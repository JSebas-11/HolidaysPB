using HolidaysPB.Domain.Entities;

namespace HolidaysPB.Application.Features.Holidays.Calculation.Strategies;

internal sealed class FridayBridgeLawHolidayCalculationStrategy : IHolidayCalculationStrategy {
    public DateOnly Calculate(Holiday holiday, int year) {
        var date = new DateOnly(year, holiday.Month, holiday.Day);
        var weekDay = date.DayOfWeek;

        var daysCalculation = weekDay switch {
            // To reach previous monday
            DayOfWeek.Tuesday => -1,
            // To reach next friday
            DayOfWeek.Wednesday => 2,
            DayOfWeek.Thursday => 1,
            // To reach previous friday
            DayOfWeek.Saturday => -1,
            // To reach next monday
            DayOfWeek.Sunday => 1,
            // Same day (Monday or Friday)
            _ => 0
        };

        return date.AddDays(daysCalculation);
    }
}