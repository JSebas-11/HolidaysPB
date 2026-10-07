using HolidaysPB.Core.Persistence.Repositories;
using HolidaysPB.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HolidaysPB.Infrastructure.Persitence.Repositories;

public sealed class HolidayRepository : IHolidayRepository {
    private readonly HolidaysDBContext _context;
    public HolidayRepository(HolidaysDBContext context) => _context = context;

    public void Add(Holiday entity) => _context.Holidays.Add(entity);
    public void Delete(Holiday entity) => _context.Holidays.Remove(entity);

    public async Task<Holiday?> GetByIdAsync(int id, CancellationToken ct)
        => await _context.Holidays.FindAsync([id], cancellationToken: ct);
    public Task<Holiday?> GetReadOnlyByIdAsync(int id, CancellationToken ct)
        => _context.Holidays
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == id, ct);

    public async Task<IReadOnlyList<Holiday>> GetAllAsync(CancellationToken ct)
        => await _context.Holidays
            .AsNoTracking()
            .ToListAsync(ct);
    public async Task<IReadOnlyList<Holiday>> GetAllByCountryAsync(int countryId, CancellationToken ct)
        => await _context.Holidays
            .AsNoTracking()
            .Where(h => h.CountryId == countryId)
            .ToListAsync(ct);
    public async Task<IReadOnlyList<Holiday>> GetAllByTypeAsync(int typeId, CancellationToken ct)
        => await _context.Holidays
            .AsNoTracking()
            .Where(h => h.TypeId == typeId)
            .ToListAsync(ct);

    public Task<bool> HasHolidaysByCountryAsync(int countryId, CancellationToken ct)
        => _context.Holidays.AnyAsync(h => h.CountryId == countryId, ct);
    public Task<bool> HasHolidaysByTypeAsync(int typeId, CancellationToken ct)
        => _context.Holidays.AnyAsync(h => h.TypeId == typeId, ct);
}