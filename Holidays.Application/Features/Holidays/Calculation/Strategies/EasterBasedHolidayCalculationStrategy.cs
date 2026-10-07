using HolidaysPB.Application.Features.Holidays.Calculation.Utils;
using HolidaysPB.Domain.Entities;

namespace HolidaysPB.Application.Features.Holidays.Calculation.Strategies;

internal sealed class EasterBasedHolidayCalculationStrategy : IHolidayCalculationStrategy {
    public DateOnly Calculate(Holiday holiday, int year)
        => HolidayDateHelper.GetEasterSunday(year).AddDays(holiday.EasterDays);
}