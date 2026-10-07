using HolidaysPB.Domain.Entities;

namespace HolidaysPB.Application.Features.Holidays.Calculation.Strategies;

internal sealed class FixedHolidayCalculationStrategy : IHolidayCalculationStrategy {
    public DateOnly Calculate(Holiday holiday, int year)
        => new (year, holiday.Month, holiday.Day);
}