using HolidaysPB.Core.Services.Country;
using HolidaysPB.Core.Services.HolidayType;

namespace HolidaysPB.Core.Services.Holiday;

public sealed record HolidayDetails(
    int Id, string Name, 
    int Day, int Month, int EasterDays, 
    CountryOverview Country, HolidayTypeOverview Type
);
public sealed record HolidayOverview(
    int Id, string Name, 
    int Day, int Month, int EasterDays, 
    int CountryId, int TypeId
);

public sealed record HolidayDateOverview(int Id, string Holiday, DateOnly Date);

public sealed record RelatedHolidaySummary(int Id, string Name);