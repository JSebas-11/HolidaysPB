using HolidaysPB.Domain.Entities;

namespace HolidaysPB.Application.Features.Holidays.Calculation;

public interface IHolidayCalculationStrategy {
    DateOnly Calculate(Holiday holiday, int year);
}