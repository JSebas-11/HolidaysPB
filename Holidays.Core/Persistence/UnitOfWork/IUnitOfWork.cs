namespace HolidaysPB.Core.Persistence.UnitOfWork;

public interface IUnitOfWork {
    Task SaveChangesAsync(CancellationToken ct);
}