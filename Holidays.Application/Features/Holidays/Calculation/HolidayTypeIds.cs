namespace HolidaysPB.Application.Features.Holidays.Calculation;

// Must match seed HolidayType ids (DML_Festivos.sql)
internal static class HolidayTypeIds {
    internal const int Fixed = 1;
    internal const int BridgeLaw = 2;
    internal const int EasterBased = 3;
    internal const int EasterBasedAndBridgeLaw = 4;
    internal const int FridayBridgeLaw = 5;

    internal static bool IsEasterBased(int typeId)
        => typeId is EasterBased or EasterBasedAndBridgeLaw;
}
