using HolidaysPB.Core.Services.Holiday;

namespace HolidaysPB.Core.Services.Country;

public sealed record CountryDetails(int Id, string Name, IReadOnlyList<RelatedHolidaySummary> Holidays);
public sealed record CountryOverview(int Id, string Name);