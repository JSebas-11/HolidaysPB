using HolidaysPB.Core.Common.Result;
using HolidaysPB.Core.Persistence.Repositories;
using HolidaysPB.Core.Persistence.UnitOfWork;
using HolidaysPB.Core.Services;
using HolidaysPB.Domain.Entities;

namespace HolidaysPB.Application.Features.HolidayTypes;

public sealed class HolidayTypeService : ICrudService<HolidayType> {
    // INITIALIZATION
    private readonly IRepository<HolidayType> _holidayTypeRepo;
    private readonly IUnitOfWork _uow;
    public HolidayTypeService(IRepository<HolidayType> holidayTypeRepo, IUnitOfWork uow) {
        _holidayTypeRepo = holidayTypeRepo;
        _uow = uow;
    }

    // METHS
    public async Task<Result<int>> AddAsync(HolidayType entity, CancellationToken ct) {
        _holidayTypeRepo.Add(entity);
        await _uow.SaveChangesAsync(ct);

        return Result<int>.Ok(entity.Id);
    }

    public async Task<Result<HolidayType>> GetByIdAsync(int id, CancellationToken ct) {
        var holyType = await _holidayTypeRepo.GetReadOnlyByIdAsync(id, ct);
        return holyType is null 
            ? Result<HolidayType>.Fail(AppError.NotFound("Holiday Type", id)) 
            : Result<HolidayType>.Ok(holyType);
    }
    public async Task<Result<IReadOnlyList<HolidayType>>> GetAllAsync(CancellationToken ct)
        => Result<IReadOnlyList<HolidayType>>.Ok(await _holidayTypeRepo.GetAllAsync(ct));

    public async Task<Result> UpdateAsync(HolidayType entity, CancellationToken ct) {
        var holyType = await _holidayTypeRepo.GetByIdAsync(entity.Id, ct);
        if (holyType is null)
            return Result.Fail(AppError.NotFound("Holiday Type", entity.Id));

        holyType.Copy(entity);
        _holidayTypeRepo.Update(holyType);
        await _uow.SaveChangesAsync(ct);

        return Result.Ok();
    }
    
    public async Task<Result> DeleteAsync(int id, CancellationToken ct) {
        var holyType = await _holidayTypeRepo.GetByIdAsync(id, ct);
        if (holyType is null)
            return Result.Fail(AppError.NotFound("Holiday Type", id));

        _holidayTypeRepo.Delete(holyType);
        await _uow.SaveChangesAsync(ct);

        return Result.Ok();
    }
}