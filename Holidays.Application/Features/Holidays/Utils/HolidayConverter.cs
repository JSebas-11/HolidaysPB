using HolidaysPB.Core.Common.Result;
using HolidaysPB.Core.Services.Holiday;
using HolidaysPB.Domain.Constants;
using HolidaysPB.Domain.Entities;

namespace HolidaysPB.Application.Features.Holidays.Utils;

internal static class HolidayConverter {
    private record HolidayRequest(string Name, int Day, int Month);

    internal static Result<Holiday> ToEntity(CreateHolidayRequest request) {
        var result = NormalizeAndValidateRequest(new (request.Name, request.Day, request.Month));
        if (result.IsFailure)
            return Result<Holiday>.Fail(result.Error!);

        var normalizedRequest = result.Value!;
        return Result<Holiday>.Ok(new () { 
                Name = normalizedRequest.Name, Day = normalizedRequest.Day, Month = normalizedRequest.Month, 
                EasterDays = request.EasterDays, TypeId = request.TypeId, CountryId = request.CountryId 
            }
        );
    }
    internal static Result<Holiday> ToEntity(int id, UpdateHolidayRequest request) {
        var result = NormalizeAndValidateRequest(new (request.Name, request.Day, request.Month));
        if (result.IsFailure)
            return Result<Holiday>.Fail(result.Error!);

        var normalizedRequest = result.Value!;
        return Result<Holiday>.Ok(new () { 
                Id = id, Name = normalizedRequest.Name, 
                Day = normalizedRequest.Day, Month = normalizedRequest.Month, EasterDays = request.EasterDays, 
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

        if (request.Day <= 0 || request.Day > 31)
            return Result<HolidayRequest>.Fail(AppError.Validation("Day must be between 1 and 31."));
        if (request.Month <= 0 || request.Month > 12)
            return Result<HolidayRequest>.Fail(AppError.Validation("Month must be between 1 and 12."));

        return Result<HolidayRequest>.Ok(new (name, request.Day, request.Month));
    }
}