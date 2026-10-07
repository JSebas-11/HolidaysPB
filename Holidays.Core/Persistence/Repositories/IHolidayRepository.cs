using HolidaysPB.Domain.Entities;

namespace HolidaysPB.Core.Persistence.Repositories;

public interface IHolidayRepository : IRepository<Holiday> {
    Task<bool> HasHolidaysByCountryAsync(int countryId, CancellationToken ct);
    Task<bool> HasHolidaysByTypeAsync(int typeId, CancellationToken ct);
    
    Task<IReadOnlyList<Holiday>> GetAllByCountryAsync(int countryId, CancellationToken ct);
    Task<IReadOnlyList<Holiday>> GetAllByTypeAsync(int typeId, CancellationToken ct);
}