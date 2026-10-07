using HolidaysPB.Application.Features.Holidays.Calculation.Utils;
using HolidaysPB.Domain.Entities;

namespace HolidaysPB.Application.Features.Holidays.Calculation.Strategies;

internal sealed class HolidayBridgeHolidayCalculationStrategy : IHolidayCalculationStrategy {
    public DateOnly Calculate(Holiday holiday, int year)
        => HolidayDateHelper.NextMonday(new (year, holiday.Month, holiday.Day));
}