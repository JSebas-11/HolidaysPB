using HolidaysPB.Core.Common.Result;
using HolidaysPB.Domain.Entities;

namespace HolidaysPB.Core.Services;

public sealed record HolidayFilter(int CountryId, int? Year);

public interface IHolidayService : ICrudService<Holiday> {
    Task<Result<IReadOnlyList<Holiday>>> GetByCountryAsync(HolidayFilter filter, CancellationToken ct);

    Task<Result<bool>> IsHolidayAsync(string date, int countryId, CancellationToken ct);
}