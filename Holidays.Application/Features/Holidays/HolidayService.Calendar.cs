using HolidaysPB.Application.Features.Holidays.Calculation;
using HolidaysPB.Application.Features.Holidays.Utils;
using HolidaysPB.Core.Common.Result;
using HolidaysPB.Core.Services.Holiday;
using HolidaysPB.Domain.Entities;

namespace HolidaysPB.Application.Features.Holidays;

public sealed partial class HolidayService : IHolidayService {
    // CALENDAR METHS
    public async Task<Result<IReadOnlyList<HolidayDateOverview>>> GetByCountryAsync(HolidayFilterRequest request, CancellationToken ct) {
        var validationResult = HolidayConverter.ValidateFilterRequest(request);
        if (!validationResult.IsSuccess)
            return Result<IReadOnlyList<HolidayDateOverview>>.Fail(validationResult.Error!);

        var country = await _countryRepo.GetReadOnlyByIdAsync(request.CountryId, ct);
        if (country is null)
            return Result<IReadOnlyList<HolidayDateOverview>>.Fail(AppError.NotFound("Country", request.CountryId));
        
        var countryHols = await _holidayRepo.GetAllByCountryAsync(country.Id, ct);
        var yearHolsResult = CalculateYearHolidays(countryHols, request.Year);
        
        return yearHolsResult.IsSuccess
            ? Result<IReadOnlyList<HolidayDateOverview>>.Ok(yearHolsResult.Value!)
            : Result<IReadOnlyList<HolidayDateOverview>>.Fail(yearHolsResult.Error!);
    }

    public async Task<Result<bool>> IsHolidayAsync(DateOnly date, int countryId, CancellationToken ct) {
        var country = await _countryRepo.GetReadOnlyByIdAsync(countryId, ct);
        if (country is null)
            return Result<bool>.Fail(AppError.NotFound("Country", countryId));

        var countryHols = await _holidayRepo.GetAllByCountryAsync(countryId, ct);
        var yearHolsResult = CalculateYearHolidays(countryHols, date.Year);
        if (yearHolsResult.IsFailure)
            return Result<bool>.Fail(yearHolsResult.Error!);

        var yearHolidays = yearHolsResult.Value!;
        return Result<bool>.Ok(yearHolidays.Any(h => h.Date == date));
    }

    // INNER METHS
    private static Result<IReadOnlyList<HolidayDateOverview>> CalculateYearHolidays(IReadOnlyList<Holiday> holidays, int year) {
        var yearHolidays = new List<HolidayDateOverview>(holidays.Count);
        foreach (var holiday in holidays) {
            var result = HolidayCalculationFactory.GetStrategy(holiday.TypeId);
            if (result.IsFailure)
                return Result<IReadOnlyList<HolidayDateOverview>>.Fail(result.Error!);

            var date = result.Value!.Calculate(holiday, year);
            yearHolidays.Add(HolidayMapper.ToDateOverview(holiday, date));
        }

        return Result<IReadOnlyList<HolidayDateOverview>>.Ok(yearHolidays);
    }
}