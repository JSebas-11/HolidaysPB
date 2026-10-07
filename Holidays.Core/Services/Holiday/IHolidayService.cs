using HolidaysPB.Core.Common.Result;

namespace HolidaysPB.Core.Services.Holiday;

public interface IHolidayService {
    Task<Result<HolidayDetails>> GetByIdAsync(int id, CancellationToken ct);
    Task<Result<IReadOnlyList<HolidayOverview>>> GetAllAsync(CancellationToken ct);

    Task<Result<IReadOnlyList<HolidayOverview>>> GetByCountryAsync(HolidayFilterRequest filter, CancellationToken ct);

    Task<Result<int>> AddAsync(CreateHolidayRequest entity, CancellationToken ct);
    Task<Result> UpdateAsync(int id, UpdateHolidayRequest entity, CancellationToken ct);
    Task<Result> DeleteAsync(int id, CancellationToken ct);

    Task<Result<bool>> IsHolidayAsync(string date, int countryId, CancellationToken ct);
}