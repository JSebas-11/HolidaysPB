using HolidaysPB.Application.Common.Constants;
using HolidaysPB.Application.Features.Holidays.Calculation;
using HolidaysPB.Application.Features.Holidays.Utils;
using HolidaysPB.Core.Common.Result;
using HolidaysPB.Core.Services.Holiday;
using HolidaysPB.Domain.Entities;

namespace HolidaysPB.Application.Features.Holidays;

public sealed partial class HolidayService : IHolidayService {
    // CALENDAR METHS
    public async Task<Result<IReadOnlyList<HolidayOverview>>> GetByCountryAsync(HolidayFilterRequest request, CancellationToken ct) {
        var validationResult = HolidayConverter.ValidateFilterRequest(request);
        if (!validationResult.IsSuccess)
            return Result<IReadOnlyList<HolidayOverview>>.Fail(validationResult.Error!);

        var country = await _countryRepo.GetReadOnlyByIdAsync(request.CountryId, ct);
        if (country is null)
            return Result<IReadOnlyList<HolidayOverview>>.Fail(AppError.NotFound("Country", request.CountryId));
        
        var countryHols = await _holidayRepo.GetAllByCountryAsync(country.Id, ct);
        if (request.Year is null)
            return Result<IReadOnlyList<HolidayOverview>>.Ok(HolidayMapper.ToOverview(countryHols));
        
        var yearHolsResult = CalculateYearHolidays(countryHols, (int)request.Year);
        
        return yearHolsResult.IsSuccess
            ? Result<IReadOnlyList<HolidayOverview>>.Ok(yearHolsResult.Value!)
            : Result<IReadOnlyList<HolidayOverview>>.Fail(yearHolsResult.Error!);
    }

    public async Task<Result<bool>> IsHolidayAsync(string date, int countryId, CancellationToken ct) {
        if (!DateOnly.TryParseExact(date, ApplicationConstants.DateFormat, out DateOnly validDate))
            return Result<bool>.Fail(AppError.Validation($"Provided date is not valid ({date})."));

        var country = await _countryRepo.GetReadOnlyByIdAsync(countryId, ct);
        if (country is null)
            return Result<bool>.Fail(AppError.NotFound("Country", countryId));

        var countryHols = await _holidayRepo.GetAllByCountryAsync(countryId, ct);
        var yearHolsResult = CalculateYearHolidays(countryHols, validDate.Year);
        if (yearHolsResult.IsFailure)
            return Result<bool>.Fail(yearHolsResult.Error!);

        var yearHolidays = yearHolsResult.Value!;
        return Result<bool>.Ok(yearHolidays.Any(h => h.Day == validDate.Day && h.Month == validDate.Month));
    }

    // INNER METHS
    private static Result<IReadOnlyList<HolidayOverview>> CalculateYearHolidays(IReadOnlyList<Holiday> holidays, int year) {
        var yearHolidays = new List<HolidayOverview>(holidays.Count);
        foreach (var holiday in holidays) {
            var result = HolidayCalculationFactory.GetStrategy(holiday.TypeId);
            if (result.IsFailure)
                return Result<IReadOnlyList<HolidayOverview>>.Fail(result.Error!);

            var date = result.Value!.Calculate(holiday, year);
            yearHolidays.Add(HolidayMapper.ToOverview(holiday, date));
        }

        return Result<IReadOnlyList<HolidayOverview>>.Ok(yearHolidays);
    }
}