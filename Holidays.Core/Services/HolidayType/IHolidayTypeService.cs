using HolidaysPB.Core.Common.Result;

namespace HolidaysPB.Core.Services.HolidayType;

public interface IHolidayTypeService {
    Task<Result<HolidayTypeDetails>> GetByIdAsync(int id, CancellationToken ct);
    Task<Result<IReadOnlyList<HolidayTypeOverview>>> GetAllAsync(CancellationToken ct);

    Task<Result<int>> AddAsync(CreateHolidayTypeRequest request, CancellationToken ct);
    Task<Result> UpdateAsync(int id, UpdateHolidayTypeRequest request, CancellationToken ct);
    Task<Result> DeleteAsync(int id, CancellationToken ct);
}