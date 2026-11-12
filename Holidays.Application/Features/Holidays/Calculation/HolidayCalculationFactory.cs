using HolidaysPB.Application.Features.Holidays.Calculation.Strategies;
using HolidaysPB.Core.Common.Result;

namespace HolidaysPB.Application.Features.Holidays.Calculation;

internal static class HolidayCalculationFactory {
    internal static Result<IHolidayCalculationStrategy> GetStrategy(int typeId)
        => typeId switch {
            HolidayTypeIds.Fixed => Result<IHolidayCalculationStrategy>.Ok(
                new FixedHolidayCalculationStrategy()
            ),
            HolidayTypeIds.BridgeLaw => Result<IHolidayCalculationStrategy>.Ok(
                new HolidayBridgeHolidayCalculationStrategy()
            ),
            HolidayTypeIds.EasterBased => Result<IHolidayCalculationStrategy>.Ok(
                new EasterBasedHolidayCalculationStrategy()
            ),
            HolidayTypeIds.EasterBasedAndBridgeLaw => Result<IHolidayCalculationStrategy>.Ok(
                new EasterBasedAndBridgeLawHolidayCalculationStrategy()
            ),
            HolidayTypeIds.FridayBridgeLaw => Result<IHolidayCalculationStrategy>.Ok(
                new FridayBridgeLawHolidayCalculationStrategy()
            ),
            _ => Result<IHolidayCalculationStrategy>.Fail(
                AppError.Conflict($"Holiday calculation strategy for type ({typeId}) is not implemented.")
            )
        };
}