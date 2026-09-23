using HolidaysPB.Core.Persistence.Repositories;
using HolidaysPB.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HolidaysPB.Infrastructure.Persitence.Repositories;

public sealed class HolidayRepository : IHolidayRepository {
    private readonly HolidaysDBContext _context;
    public HolidayRepository(HolidaysDBContext context) => _context = context;

    public void Add(Holiday entity) => _context.Holidays.Add(entity);
    public void Update(Holiday entity) => _context.Holidays.Update(entity);
    public void Delete(Holiday entity) => _context.Holidays.Remove(entity);

    public Task<Holiday?> GetByIdAsync(int id, CancellationToken ct)
        => _context.Holidays
            .Include(c => c.HolidayType)
            .Include(c => c.Country)
            .FirstOrDefaultAsync(h => h.Id == id, ct);
    public Task<Holiday?> GetReadOnlyByIdAsync(int id, CancellationToken ct)
        => _context.Holidays
            .AsNoTracking()
            .Include(c => c.HolidayType)
            .Include(c => c.Country)
            .FirstOrDefaultAsync(h => h.Id == id, ct);

    public async Task<IReadOnlyList<Holiday>> GetAllAsync(CancellationToken ct)
        => await _context.Holidays
            .AsNoTracking()
            .Include(c => c.HolidayType)
            .Include(c => c.Country)
            .ToListAsync(ct);
    public async Task<IReadOnlyList<Holiday>> GetAllByCountry(int countryId, CancellationToken ct)
        => await _context.Holidays
            .AsNoTracking()
            .Include(c => c.HolidayType)
            .Include(c => c.Country)
            .Where(h => h.CountryId == countryId)
            .ToListAsync(ct);
}