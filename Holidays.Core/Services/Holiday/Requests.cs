namespace HolidaysPB.Core.Services.Holiday;

public sealed record CreateHolidayRequest(
    string Name, int Day, int Month, int EasterDays,
    int CountryId, int TypeId
);
public sealed record UpdateHolidayRequest(
    string Name, int Day, int Month, int EasterDays,
    int CountryId, int TypeId
);

public sealed record HolidayFilterRequest(int CountryId, int? Year);