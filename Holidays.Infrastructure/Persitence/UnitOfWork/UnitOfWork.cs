using HolidaysPB.Core.Persistence.UnitOfWork;

namespace HolidaysPB.Infrastructure.Persitence.UnitOfWork;

public sealed class UnitOfWork : IUnitOfWork {
    private readonly HolidaysDBContext _context;
    public UnitOfWork(HolidaysDBContext context) => _context = context;

    public Task SaveChangesAsync(CancellationToken ct)
        => _context.SaveChangesAsync(ct);
}