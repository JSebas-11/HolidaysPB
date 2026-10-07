using HolidaysPB.Core.Services.Holiday;
using HolidaysPB.Core.Services.HolidayType;
using HolidaysPB.Domain.Entities;

namespace HolidaysPB.Application.Features.HolidayTypes.Utils;

internal static class HolidayTypeMapper {
    internal static HolidayTypeDetails ToDetails(HolidayType type, IReadOnlyList<RelatedHolidaySummary> holidays)
        => new (type.Id, type.Type, holidays);
    internal static IReadOnlyList<HolidayTypeOverview> ToOverview(IReadOnlyList<HolidayType> types)
        => [.. types.Select(ToOverview)];
    internal static HolidayTypeOverview ToOverview(HolidayType type)
        => new (type.Id, type.Type);
}