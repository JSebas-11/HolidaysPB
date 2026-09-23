using HolidaysPB.Domain.Entities;

namespace HolidaysPB.Core.Persistence.Repositories;

public interface IHolidayRepository : IRepository<Holiday> {
    Task<IReadOnlyList<Holiday>> GetAllByCountry(int countryId, CancellationToken ct);
}