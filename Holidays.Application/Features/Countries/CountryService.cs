using HolidaysPB.Application.Features.Countries.Utils;
using HolidaysPB.Application.Features.Holidays.Services;
using HolidaysPB.Core.Common.Result;
using HolidaysPB.Core.Persistence.Repositories;
using HolidaysPB.Core.Persistence.UnitOfWork;
using HolidaysPB.Core.Services.Country;
using HolidaysPB.Domain.Entities;

namespace HolidaysPB.Application.Features.Countries;

public sealed class CountryService : ICountryService {
    // INITIALIZATION
    private readonly IRepository<Country> _countryRepo;
    private readonly RelatedHolidayService _relatedHolidaySvc;
    private readonly IUnitOfWork _uow;
    public CountryService(IRepository<Country> countryRepo, RelatedHolidayService relatedHolidaySvc, IUnitOfWork uow) {
        _countryRepo = countryRepo;
        _relatedHolidaySvc = relatedHolidaySvc;
        _uow = uow;
    }

    // METHS
    public async Task<Result<int>> AddAsync(CreateCountryRequest request, CancellationToken ct) {
        var conversionResult = CountryConverter.ToEntity(request);
        if (!conversionResult.IsSuccess)
            return Result<int>.Fail(conversionResult.Error!);

        var country = conversionResult.Value!;
        _countryRepo.Add(country);
        await _uow.SaveChangesAsync(ct);

        return Result<int>.Ok(country.Id);
    }

    public async Task<Result<CountryDetails>> GetByIdAsync(int id, CancellationToken ct) {
        var country = await _countryRepo.GetReadOnlyByIdAsync(id, ct);
        if (country is null)
            return Result<CountryDetails>.Fail(AppError.NotFound("Country", id));

        var countryHols = await _relatedHolidaySvc.GetRelatedByCountryAsync(id, ct);
        return Result<CountryDetails>.Ok(CountryMapper.ToDetails(country, countryHols));
    }
    public async Task<Result<IReadOnlyList<CountryOverview>>> GetAllAsync(CancellationToken ct) {
        var countries = await _countryRepo.GetAllAsync(ct);
        return Result<IReadOnlyList<CountryOverview>>.Ok(CountryMapper.ToOverview(countries));
    }

    public async Task<Result> UpdateAsync(int id, UpdateCountryRequest request, CancellationToken ct) {
        var conversionResult = CountryConverter.ToEntity(id, request);
        if (!conversionResult.IsSuccess)
            return Result.Fail(conversionResult.Error!);

        var country = await _countryRepo.GetByIdAsync(id, ct); // Returns a tracked entity
        if (country is null)
            return Result.Fail(AppError.NotFound("Country", id));

        country.Copy(conversionResult.Value!);
        await _uow.SaveChangesAsync(ct);

        return Result.Ok();
    }
    
    public async Task<Result> DeleteAsync(int id, CancellationToken ct) {
        var country = await _countryRepo.GetByIdAsync(id, ct);
        if (country is null)
            return Result.Fail(AppError.NotFound("Country", id));

        _countryRepo.Delete(country);
        await _uow.SaveChangesAsync(ct);

        return Result.Ok();
    }
}