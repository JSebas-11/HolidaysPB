using HolidaysPB.Core.Common.Result;

namespace HolidaysPB.Core.Services.Country;

public interface ICountryService {
    Task<Result<CountryDetails>> GetByIdAsync(int id, CancellationToken ct);
    Task<Result<IReadOnlyList<CountryOverview>>> GetAllAsync(CancellationToken ct);

    Task<Result<int>> AddAsync(CreateCountryRequest entity, CancellationToken ct);
    Task<Result> UpdateAsync(int id, UpdateCountryRequest entity, CancellationToken ct);
    Task<Result> DeleteAsync(int id, CancellationToken ct);
}