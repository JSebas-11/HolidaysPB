using HolidaysPB.Application.Features.Holidays.Services;
using HolidaysPB.Application.Features.HolidayTypes.Utils;
using HolidaysPB.Core.Common.Result;
using HolidaysPB.Core.Persistence.Repositories;
using HolidaysPB.Core.Persistence.UnitOfWork;
using HolidaysPB.Core.Services.HolidayType;
using HolidaysPB.Domain.Entities;

namespace HolidaysPB.Application.Features.HolidayTypes;

public sealed class HolidayTypeService : IHolidayTypeService {
    // INITIALIZATION
    private readonly IRepository<HolidayType> _holidayTypeRepo;
    private readonly IHolidayRepository _holidayRepo;
    private readonly RelatedHolidayService _relatedHolidaySvc;
    private readonly IUnitOfWork _uow;
    public HolidayTypeService(
        IRepository<HolidayType> holidayTypeRepo, IHolidayRepository holidayRepo,
        RelatedHolidayService relatedHolidaySvc, IUnitOfWork uow
    ) {
        _holidayTypeRepo = holidayTypeRepo;
        _holidayRepo = holidayRepo;
        _relatedHolidaySvc = relatedHolidaySvc;
        _uow = uow;
    }

    // METHS
    public async Task<Result<int>> AddAsync(CreateHolidayTypeRequest request, CancellationToken ct) {
        var conversionResult = HolidayTypeConverter.ToEntity(request);
        if (!conversionResult.IsSuccess)
            return Result<int>.Fail(conversionResult.Error!);

        var holidayType = conversionResult.Value!;
        _holidayTypeRepo.Add(holidayType);
        await _uow.SaveChangesAsync(ct);

        return Result<int>.Ok(holidayType.Id);
    }

    public async Task<Result<HolidayTypeDetails>> GetByIdAsync(int id, CancellationToken ct) {
        var holyType = await _holidayTypeRepo.GetReadOnlyByIdAsync(id, ct);
        if (holyType is null)
            return Result<HolidayTypeDetails>.Fail(AppError.NotFound("Holiday Type", id));

        var typeHols = await _relatedHolidaySvc.GetRelatedByTypeAsync(id, ct);
        return Result<HolidayTypeDetails>.Ok(HolidayTypeMapper.ToDetails(holyType, typeHols));
    }
    public async Task<Result<IReadOnlyList<HolidayTypeOverview>>> GetAllAsync(CancellationToken ct) {
        var types = await _holidayTypeRepo.GetAllAsync(ct);
        return Result<IReadOnlyList<HolidayTypeOverview>>.Ok(HolidayTypeMapper.ToOverview(types));
    }

    public async Task<Result> UpdateAsync(int id, UpdateHolidayTypeRequest request, CancellationToken ct) {
        var conversionResult = HolidayTypeConverter.ToEntity(id, request);
        if (!conversionResult.IsSuccess)
            return Result.Fail(conversionResult.Error!);

        var holyType = await _holidayTypeRepo.GetByIdAsync(id, ct); // Returns a tracked entity
        if (holyType is null)
            return Result.Fail(AppError.NotFound("Holiday Type", id));

        holyType.Copy(conversionResult.Value!);
        await _uow.SaveChangesAsync(ct);

        return Result.Ok();
    }
    
    public async Task<Result> DeleteAsync(int id, CancellationToken ct) {
        var holyType = await _holidayTypeRepo.GetByIdAsync(id, ct);
        if (holyType is null)
            return Result.Fail(AppError.NotFound("Holiday Type", id));

        if (await _holidayRepo.HasHolidaysByTypeAsync(id, ct))
            return Result.Fail(AppError.Conflict($"Holiday Type ({id}) cannot be deleted because it has holidays."));

        _holidayTypeRepo.Delete(holyType);
        await _uow.SaveChangesAsync(ct);

        return Result.Ok();
    }
}