using HolidaysPB.Application.Features.Holidays.Calculation;
using HolidaysPB.Core.Common.Result;
using HolidaysPB.Core.Services.Holiday;
using HolidaysPB.Domain.Constants;
using HolidaysPB.Domain.Entities;

namespace HolidaysPB.Application.Features.Holidays.Utils;

internal static class HolidayConverter {
    private record HolidayRequest(string Name, int Day, int Month, int EasterDays, int TypeId);

    // Non-leap year: Feb 29 is rejected because it can't be computed every year
    private const int DayValidationYear = 2001;

    internal static Result<Holiday> ToEntity(CreateHolidayRequest request) {
        var result = NormalizeAndValidateRequest(
            new (request.Name, request.Day, request.Month, request.EasterDays, request.TypeId)
        );
        if (result.IsFailure)
            return Result<Holiday>.Fail(result.Error!);

        var normalizedRequest = result.Value!;
        return Result<Holiday>.Ok(new () {
                Name = normalizedRequest.Name, Day = normalizedRequest.Day, Month = normalizedRequest.Month,
                EasterDays = normalizedRequest.EasterDays, TypeId = request.TypeId, CountryId = request.CountryId
            }
        );
    }
    internal static Result<Holiday> ToEntity(int id, UpdateHolidayRequest request) {
        var result = NormalizeAndValidateRequest(
            new (request.Name, request.Day, request.Month, request.EasterDays, request.TypeId)
        );
        if (result.IsFailure)
            return Result<Holiday>.Fail(result.Error!);

        var normalizedRequest = result.Value!;
        return Result<Holiday>.Ok(new () {
                Id = id, Name = normalizedRequest.Name,
                Day = normalizedRequest.Day, Month = normalizedRequest.Month, EasterDays = normalizedRequest.EasterDays,
                TypeId = request.TypeId, CountryId = request.CountryId
            }
        );
    }

    internal static Result<HolidayFilterRequest> ValidateFilterRequest(HolidayFilterRequest request) {
        if (request.Year is <= 0 or > 9999)
            return Result<HolidayFilterRequest>.Fail(AppError.Validation("Year must be between 1 and 9999."));

        return Result<HolidayFilterRequest>.Ok(request);
    }

    // INNER METHS
    private static Result<HolidayRequest> NormalizeAndValidateRequest(HolidayRequest request) {
        var name = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return Result<HolidayRequest>.Fail(AppError.Validation("Holiday name cannot be empty."));
        if (name.Length > DomainConstants.Length.Holiday.MaxName)
            return Result<HolidayRequest>.Fail(AppError.Validation(DomainConstants.Length.Holiday.MaxNameErrorMsg));

        // Easter-based dates only depend on EasterDays: Day/Month are stored as 0 (same as seed data)
        if (HolidayTypeIds.IsEasterBased(request.TypeId))
            return Result<HolidayRequest>.Ok(request with { Name = name, Day = 0, Month = 0 });

        if (request.Month is < 1 or > 12)
            return Result<HolidayRequest>.Fail(AppError.Validation("Month must be between 1 and 12."));

        var maxDay = DateTime.DaysInMonth(DayValidationYear, request.Month);
        if (request.Day < 1 || request.Day > maxDay)
            return Result<HolidayRequest>.Fail(
                AppError.Validation($"Day must be between 1 and {maxDay} for month {request.Month}.")
            );

        // Date-based types ignore EasterDays
        return Result<HolidayRequest>.Ok(request with { Name = name, EasterDays = 0 });
    }
}
