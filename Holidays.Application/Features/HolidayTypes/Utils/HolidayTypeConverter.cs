using HolidaysPB.Core.Common.Result;
using HolidaysPB.Core.Services.HolidayType;
using HolidaysPB.Domain.Constants;
using HolidaysPB.Domain.Entities;

namespace HolidaysPB.Application.Features.HolidayTypes.Utils;

internal static class HolidayTypeConverter {
    public static Result<HolidayType> ToEntity(CreateHolidayTypeRequest request) {
        var result = NormalizeAndValidateType(request.Type);
        return result.IsSuccess
            ? Result<HolidayType>.Ok(new() { Type = result.Value! })
            : Result<HolidayType>.Fail(result.Error!);
    }
    public static Result<HolidayType> ToEntity(int id, UpdateHolidayTypeRequest request) {
        var result = NormalizeAndValidateType(request.Type);
        return result.IsSuccess
            ? Result<HolidayType>.Ok(new() { Id = id, Type = result.Value! })
            : Result<HolidayType>.Fail(result.Error!);
    }

    // INNER METH
    private static Result<string> NormalizeAndValidateType(string type) {
        type = type.Trim();

        if (string.IsNullOrWhiteSpace(type))
            return Result<string>.Fail(AppError.Validation("Holiday type cannot be empty."));
        if (type.Length > DomainConstants.Length.HolidayType.MaxType)
            return Result<string>.Fail(AppError.Validation(DomainConstants.Length.HolidayType.MaxTypeErrorMsg));

        return Result<string>.Ok(type);
    }
}