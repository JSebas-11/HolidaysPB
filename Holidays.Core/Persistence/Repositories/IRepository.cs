namespace HolidaysPB.Core.Persistence.Repositories;

public interface IRepository<T> where T : class {
    Task<T?> GetByIdAsync(int id, CancellationToken ct); // Tracked
    Task<T?> GetReadOnlyByIdAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct);

    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
}