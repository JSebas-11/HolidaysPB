using HolidaysPB.Core.Common.Result;

namespace HolidaysPB.Core.Services;

// PENDING DTO across entities services
public interface ICrudService<T> where T : class {
    Task<Result<T>> GetByIdAsync(int id, CancellationToken ct);
    Task<Result<IReadOnlyList<T>>> GetAllAsync(CancellationToken ct);

    Task<Result<int>> AddAsync(T entity, CancellationToken ct);
    Task<Result> UpdateAsync(T entity, CancellationToken ct);
    Task<Result> DeleteAsync(int id, CancellationToken ct);
}