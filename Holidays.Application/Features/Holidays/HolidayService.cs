using HolidaysPB.Application.Common.Constants;
using HolidaysPB.Application.Features.Countries.Utils;
using HolidaysPB.Application.Features.Holidays.Utils;
using HolidaysPB.Application.Features.HolidayTypes.Utils;
using HolidaysPB.Core.Common.Extensions;
using HolidaysPB.Core.Common.Result;
using HolidaysPB.Core.Persistence.Repositories;
using HolidaysPB.Core.Persistence.UnitOfWork;
using HolidaysPB.Core.Services.Holiday;
using HolidaysPB.Domain.Entities;

namespace HolidaysPB.Application.Features.Holidays;

public sealed class HolidayService : IHolidayService {
    private record RelatedEntities(Country Country, HolidayType Type);

    // INITIALIZATION
    private readonly IHolidayRepository _holidayRepo;
    private readonly IRepository<Country> _countryRepo;
    private readonly IRepository<HolidayType> _typeRepo;
    private readonly IUnitOfWork _uow;
    public HolidayService(IHolidayRepository holidayRepo, IRepository<Country> countryRepo, IRepository<HolidayType> typeRepo, IUnitOfWork uow) {
        _holidayRepo = holidayRepo;
        _countryRepo = countryRepo;
        _typeRepo = typeRepo;
        _uow = uow;
    }

    // METHS
    public async Task<Result<int>> AddAsync(CreateHolidayRequest request, CancellationToken ct) {
        var conversionResult = HolidayConverter.ToEntity(request);
        if (!conversionResult.IsSuccess)
            return Result<int>.Fail(conversionResult.Error!);

        var relatedResult = await FetchAndValidateRelatedAsync(null, request.CountryId, request.TypeId, false, ct);
        if (relatedResult.IsFailure)
            return Result<int>.Fail(relatedResult.Error!);

        var holiday = conversionResult.Value!;
        _holidayRepo.Add(holiday);
        await _uow.SaveChangesAsync(ct);

        return Result<int>.Ok(holiday.Id);
    }

    public async Task<Result<HolidayDetails>> GetByIdAsync(int id, CancellationToken ct) {
        var holiday = await _holidayRepo.GetReadOnlyByIdAsync(id, ct);
        if (holiday is null)
            return Result<HolidayDetails>.Fail(AppError.NotFound("Holiday", id));

        var relatedResult = await FetchAndValidateRelatedAsync(id, holiday.CountryId, holiday.TypeId, true, ct);
        if (relatedResult.IsFailure)
            return Result<HolidayDetails>.Fail(relatedResult.Error!);

        return Result<HolidayDetails>.Ok(CreateDetails(holiday, relatedResult.Value!));
    }
    public async Task<Result<IReadOnlyList<HolidayOverview>>> GetAllAsync(CancellationToken ct) {
        var holidays = await _holidayRepo.GetAllAsync(ct);
        return Result<IReadOnlyList<HolidayOverview>>.Ok(HolidayMapper.ToOverview(holidays));
    }

    public async Task<Result> UpdateAsync(int id, UpdateHolidayRequest request, CancellationToken ct) {
        var conversionResult = HolidayConverter.ToEntity(id, request);
        if (!conversionResult.IsSuccess)
            return Result.Fail(conversionResult.Error!);

        var holiday = await _holidayRepo.GetByIdAsync(id, ct); // Returns a tracked entity
        if (holiday is null)
            return Result.Fail(AppError.NotFound("Holiday", id));

        var relatedResult = await FetchAndValidateRelatedAsync(id, request.CountryId, request.TypeId, false, ct);
        if (relatedResult.IsFailure)
            return Result.Fail(relatedResult.Error!);

        holiday.Copy(conversionResult.Value!);
        await _uow.SaveChangesAsync(ct);

        return Result.Ok();
    }
    
    public async Task<Result> DeleteAsync(int id, CancellationToken ct) {
        var holiday = await _holidayRepo.GetByIdAsync(id, ct);
        if (holiday is null)
            return Result.Fail(AppError.NotFound("Holiday", id));

        _holidayRepo.Delete(holiday);
        await _uow.SaveChangesAsync(ct);

        return Result.Ok();
    }

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

        return Result<IReadOnlyList<HolidayOverview>>.Ok(CalculateYearHolidays(countryHols, (int)request.Year));
    }

    public async Task<Result<bool>> IsHolidayAsync(string date, int countryId, CancellationToken ct) {
        if (!DateOnly.TryParseExact(date, ApplicationConstants.DateFormat, out DateOnly validDate))
            return Result<bool>.Fail(AppError.Validation($"Provided date is not valid ({date})."));

        var country = await _countryRepo.GetReadOnlyByIdAsync(countryId, ct);
        if (country is null)
            return Result<bool>.Fail(AppError.NotFound("Country", countryId));

        var countryHols = await _holidayRepo.GetAllByCountryAsync(countryId, ct);
        var yearHolidays = CalculateYearHolidays(countryHols, validDate.Year);
        return Result<bool>.Ok(yearHolidays.Any(h => h.Day == validDate.Day && h.Month == validDate.Month));
    }

    // INNER METHS
    private static IReadOnlyList<HolidayOverview> CalculateYearHolidays(IReadOnlyList<Holiday> holidays, int year) {
        throw new NotImplementedException();
    }

    private static HolidayDetails CreateDetails(Holiday holiday, RelatedEntities related)
        => HolidayMapper.ToDetails(
            holiday, CountryMapper.ToOverview(related.Country), HolidayTypeMapper.ToOverview(related.Type)
        );
    
    private async Task<Result<RelatedEntities>> FetchAndValidateRelatedAsync(
        int? holidayId, int countryId, int typeId, bool isCritical, CancellationToken ct
    ) {
        (var country, var type) = await (
            _countryRepo.GetReadOnlyByIdAsync(countryId, ct), _typeRepo.GetReadOnlyByIdAsync(typeId, ct)
        );

        if (country is null)
            return isCritical 
                ? Result<RelatedEntities>.Fail(AppError.Unexpected($"Holiday's ({holidayId}) Country is missing."))
                : Result<RelatedEntities>.Fail(AppError.Conflict($"Provided Country ({countryId}) does not exist."));
        if (type is null)
            return isCritical 
                ? Result<RelatedEntities>.Fail(AppError.Unexpected($"Holiday's ({holidayId}) Type is missing."))
                : Result<RelatedEntities>.Fail(AppError.Conflict($"Provided Type ({typeId}) does not exist."));
        
        return Result<RelatedEntities>.Ok(new (country, type));
    }
}