using HolidaysPB.Core.Common.Result;
using HolidaysPB.Core.Persistence.Repositories;
using HolidaysPB.Core.Persistence.UnitOfWork;
using HolidaysPB.Core.Services;
using HolidaysPB.Domain.Entities;

namespace HolidaysPB.Application.Features.Countries;

public sealed class CountryService : ICrudService<Country> {
    // INITIALIZATION
    private readonly IRepository<Country> _countryRepo;
    private readonly IUnitOfWork _uow;
    public CountryService(IRepository<Country> countryRepo, IUnitOfWork uow) {
        _countryRepo = countryRepo;
        _uow = uow;
    }

    // METHS
    public async Task<Result<int>> AddAsync(Country entity, CancellationToken ct) {
        _countryRepo.Add(entity);
        await _uow.SaveChangesAsync(ct);

        return Result<int>.Ok(entity.Id);
    }

    public async Task<Result<Country>> GetByIdAsync(int id, CancellationToken ct) {
        var country = await _countryRepo.GetReadOnlyByIdAsync(id, ct);
        return country is null 
            ? Result<Country>.Fail(AppError.NotFound("Country", id)) 
            : Result<Country>.Ok(country);
    }
    public async Task<Result<IReadOnlyList<Country>>> GetAllAsync(CancellationToken ct)
        => Result<IReadOnlyList<Country>>.Ok(await _countryRepo.GetAllAsync(ct));

    public async Task<Result> UpdateAsync(Country entity, CancellationToken ct) {
        var country = await _countryRepo.GetByIdAsync(entity.Id, ct);
        if (country is null)
            return Result.Fail(AppError.NotFound("Country", entity.Id));

        country.Copy(entity);
        _countryRepo.Update(country);
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