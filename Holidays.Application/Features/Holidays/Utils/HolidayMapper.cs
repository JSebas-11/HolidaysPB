using HolidaysPB.Core.Services.Country;
using HolidaysPB.Core.Services.Holiday;
using HolidaysPB.Core.Services.HolidayType;
using HolidaysPB.Domain.Entities;

namespace HolidaysPB.Application.Features.Holidays.Utils;

internal static class HolidayMapper {
    internal static HolidayDetails ToDetails(Holiday holiday, CountryOverview country, HolidayTypeOverview type)
        => new (
            holiday.Id, holiday.Name, 
            holiday.Day, holiday.Month, holiday.EasterDays,
            country, type
        );
    internal static IReadOnlyList<RelatedHolidaySummary> ToRelated(IReadOnlyList<Holiday> holidays)
        => [.. holidays.Select(h => new RelatedHolidaySummary(h.Id, h.Name))];
    internal static IReadOnlyList<HolidayOverview> ToOverview(IReadOnlyList<Holiday> holidays)
        => [.. holidays.Select(ToOverview)];
    internal static HolidayOverview ToOverview(Holiday holiday)
        => new (
            holiday.Id, holiday.Name, 
            holiday.Day, holiday.Month, holiday.EasterDays,
            holiday.CountryId, holiday.TypeId
        );

    internal static HolidayDateOverview ToDateOverview(Holiday holiday, DateOnly date)
        => new (holiday.Id, holiday.Name, date);
}