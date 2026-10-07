using HolidaysPB.Core.Common.Result;
using HolidaysPB.Core.Services.Country;
using HolidaysPB.Domain.Constants;
using HolidaysPB.Domain.Entities;

namespace HolidaysPB.Application.Features.Countries.Utils;

internal static class CountryConverter {
    internal static Result<Country> ToEntity(CreateCountryRequest request) {
        var result = NormalizeAndValidateName(request.Name);   
        return result.IsSuccess
            ? Result<Country>.Ok(new() { Name = result.Value! })
            : Result<Country>.Fail(result.Error!);
    }
    internal static Result<Country> ToEntity(int id, UpdateCountryRequest request) {
        var result = NormalizeAndValidateName(request.Name);   
        return result.IsSuccess
            ? Result<Country>.Ok(new() { Id = id, Name = result.Value! })
            : Result<Country>.Fail(result.Error!);
    }

    // INNER METHS
    private static Result<string> NormalizeAndValidateName(string name) {
        name = name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            return Result<string>.Fail(AppError.Validation("Country name cannot be empty."));
        if (name.Length > DomainConstants.Length.Country.MaxName)
            return Result<string>.Fail(AppError.Validation(DomainConstants.Length.Country.MaxNameErrorMsg));

        return Result<string>.Ok(name);
    }
}