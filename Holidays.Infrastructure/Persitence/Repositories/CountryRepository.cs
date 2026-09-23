using HolidaysPB.Core.Persistence.Repositories;
using HolidaysPB.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HolidaysPB.Infrastructure.Persitence.Repositories;

public sealed class CountryRepository : IRepository<Country> {
    private readonly HolidaysDBContext _context;
    public CountryRepository(HolidaysDBContext context) => _context = context;

    public void Add(Country entity) => _context.Countries.Add(entity);
    public void Update(Country entity) => _context.Countries.Update(entity);
    public void Delete(Country entity) => _context.Countries.Remove(entity);
        
    public async Task<Country?> GetByIdAsync(int id, CancellationToken ct)
        => await _context.Countries.FindAsync([id], cancellationToken: ct);
    public Task<Country?> GetReadOnlyByIdAsync(int id, CancellationToken ct)
        => _context.Countries
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<Country>> GetAllAsync(CancellationToken ct)
        => await _context.Countries.AsNoTracking().ToListAsync(ct);
}