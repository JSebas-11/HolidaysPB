using HolidaysPB.Application.Features.Holidays.Utils;
using HolidaysPB.Core.Persistence.Repositories;
using HolidaysPB.Core.Services.Holiday;

namespace HolidaysPB.Application.Features.Holidays.Services;

public sealed class RelatedHolidayService {
    private readonly IHolidayRepository _holidayRepo;
    public RelatedHolidayService(IHolidayRepository holidayRepo) => _holidayRepo = holidayRepo;

    // METHS
    internal async Task<IReadOnlyList<RelatedHolidaySummary>> GetRelatedByCountryAsync(int countryId, CancellationToken ct) {
        var relatedHols = await _holidayRepo.GetAllByCountryAsync(countryId, ct);
        return HolidayMapper.ToRelated(relatedHols);
    }
    internal async Task<IReadOnlyList<RelatedHolidaySummary>> GetRelatedByTypeAsync(int typeId, CancellationToken ct) {
        var relatedHols = await _holidayRepo.GetAllByTypeAsync(typeId, ct);
        return HolidayMapper.ToRelated(relatedHols);
    }
}