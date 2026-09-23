using HolidaysPB.Core.Persistence.Repositories;
using HolidaysPB.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HolidaysPB.Infrastructure.Persitence.Repositories;

public sealed class HolidayTypeRepository : IRepository<HolidayType> {
    private readonly HolidaysDBContext _context;
    public HolidayTypeRepository(HolidaysDBContext context) => _context = context;

    public void Add(HolidayType entity) => _context.HolidayTypes.Add(entity);
    public void Update(HolidayType entity) => _context.HolidayTypes.Update(entity);
    public void Delete(HolidayType entity) => _context.HolidayTypes.Remove(entity);
        
    public async Task<HolidayType?> GetByIdAsync(int id, CancellationToken ct)
        => await _context.HolidayTypes.FindAsync([id], cancellationToken: ct);
    public Task<HolidayType?> GetReadOnlyByIdAsync(int id, CancellationToken ct)
        => _context.HolidayTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(ht => ht.Id == id, ct);

    public async Task<IReadOnlyList<HolidayType>> GetAllAsync(CancellationToken ct)
        => await _context.HolidayTypes.AsNoTracking().ToListAsync(ct);
}