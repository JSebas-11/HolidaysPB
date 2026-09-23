using HolidaysPB.Core.Common.Result;
using HolidaysPB.Core.Persistence.Repositories;
using HolidaysPB.Core.Persistence.UnitOfWork;
using HolidaysPB.Core.Services;
using HolidaysPB.Domain.Entities;

namespace HolidaysPB.Application.Features.Holidays;

public sealed class HolidayService : IHolidayService {
    // INITIALIZATION
    private readonly IHolidayRepository _holidayRepo;
    private readonly IUnitOfWork _uow;
    public HolidayService(IHolidayRepository holidayRepo, IUnitOfWork uow) {
        _holidayRepo = holidayRepo;
        _uow = uow;
    }

    // METHS
    public async Task<Result<int>> AddAsync(Holiday entity, CancellationToken ct) {
        _holidayRepo.Add(entity);
        await _uow.SaveChangesAsync(ct);

        return Result<int>.Ok(entity.Id);
    }

    public async Task<Result<Holiday>> GetByIdAsync(int id, CancellationToken ct) {
        var holiday = await _holidayRepo.GetReadOnlyByIdAsync(id, ct);
        return holiday is null 
            ? Result<Holiday>.Fail(AppError.NotFound("Holiday", id)) 
            : Result<Holiday>.Ok(holiday);
    }
    public async Task<Result<IReadOnlyList<Holiday>>> GetAllAsync(CancellationToken ct)
        => Result<IReadOnlyList<Holiday>>.Ok(await _holidayRepo.GetAllAsync(ct));

    public async Task<Result> UpdateAsync(Holiday entity, CancellationToken ct) {
        var holiday = await _holidayRepo.GetByIdAsync(entity.Id, ct);
        if (holiday is null)
            return Result.Fail(AppError.NotFound("Holiday", entity.Id));

        holiday.Copy(entity);
        _holidayRepo.Update(holiday);
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

    public async Task<Result<IReadOnlyList<Holiday>>> GetByCountryAsync(HolidayFilter filter, CancellationToken ct) {
        var countryHols = await _holidayRepo.GetAllByCountryAsync(filter.CountryId, ct);
        if (filter.Year is null)
            return Result<IReadOnlyList<Holiday>>.Ok(countryHols);

        return Result<IReadOnlyList<Holiday>>.Ok(CalculateYearHolidays(countryHols, (int)filter.Year));
    }

    public async Task<Result<bool>> IsHolidayAsync(string date, int countryId, CancellationToken ct) {
        if (!DateOnly.TryParse(date, out DateOnly validDate))
            return Result<bool>.Fail(AppError.Validation($"Provided date is not valid ({date})."));

        var countryHols = await _holidayRepo.GetAllByCountryAsync(countryId, ct);
        if (countryHols.Count == 0)
            return Result<bool>.Fail(AppError.NotFound($"There aren't holidays for the given country ({countryId})."));

        var yearHolidays = CalculateYearHolidays(countryHols, validDate.Year);
        return Result<bool>.Ok(yearHolidays.Any(h => h.Day == validDate.Day && h.Month == validDate.Month));
    }

    // INNER METHS
    private static IReadOnlyList<Holiday> CalculateYearHolidays(IReadOnlyList<Holiday> holidays, int year) {
        throw new NotImplementedException();
    }
}