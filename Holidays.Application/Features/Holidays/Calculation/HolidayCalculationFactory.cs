using HolidaysPB.Application.Features.Holidays.Calculation.Strategies;
using HolidaysPB.Core.Common.Result;

namespace HolidaysPB.Application.Features.Holidays.Calculation;

internal static class HolidayCalculationFactory {
    // TypeId must match seed HolidayTypes ids
    internal static Result<IHolidayCalculationStrategy> GetStrategy(int typeId)
        => typeId switch {
            1 => Result<IHolidayCalculationStrategy>.Ok(
                new FixedHolidayCalculationStrategy()
            ),
            2 => Result<IHolidayCalculationStrategy>.Ok(
                new HolidayBridgeHolidayCalculationStrategy()
            ),
            3 => Result<IHolidayCalculationStrategy>.Ok(
                new EasterBasedHolidayCalculationStrategy()
            ),
            4 => Result<IHolidayCalculationStrategy>.Ok(
                new EasterBasedAndBridgeLawHolidayCalculationStrategy()
            ),
            5 => Result<IHolidayCalculationStrategy>.Ok(
                new FridayBridgeLawHolidayCalculationStrategy()
            ),
            _ => Result<IHolidayCalculationStrategy>.Fail(
                AppError.Conflict($"Holiday calculation strategy for type ({typeId}) is not implemented.")
            )
        };
}