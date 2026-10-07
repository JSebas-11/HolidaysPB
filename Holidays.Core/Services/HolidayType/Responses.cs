using HolidaysPB.Core.Services.Holiday;

namespace HolidaysPB.Core.Services.HolidayType;

public sealed record HolidayTypeDetails(int Id, string Type, IReadOnlyList<RelatedHolidaySummary> Holidays);
public sealed record HolidayTypeOverview(int Id, string Type);