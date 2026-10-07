using HolidaysPB.Core.Services.Country;
using HolidaysPB.Core.Services.Holiday;
using HolidaysPB.Domain.Entities;

namespace HolidaysPB.Application.Features.Countries.Utils;

internal static class CountryMapper {
    internal static CountryDetails ToDetails(Country country, IReadOnlyList<RelatedHolidaySummary> holidays)
        => new (country.Id, country.Name, holidays);
    internal static IReadOnlyList<CountryOverview> ToOverview(IReadOnlyList<Country> countries)
        => [.. countries.Select(ToOverview)];
    internal static CountryOverview ToOverview(Country country)
        => new (country.Id, country.Name);
}