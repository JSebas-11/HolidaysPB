using HolidaysPB.Application.Features.Holidays.Calculation.Utils;
using HolidaysPB.Domain.Entities;

namespace HolidaysPB.Application.Features.Holidays.Calculation.Strategies;

internal sealed class EasterBasedAndBridgeLawHolidayCalculationStrategy : IHolidayCalculationStrategy {
    public DateOnly Calculate(Holiday holiday, int year) {
        var easterDate = HolidayDateHelper.GetEasterSunday(year).AddDays(holiday.EasterDays);
        return HolidayDateHelper.NextMonday(easterDate);
    }
}