using HolidaysPB.Api.Common.Extensions;
using HolidaysPB.Core.Services.HolidayType;
using Microsoft.AspNetCore.Mvc;

namespace HolidaysPB.Api.Controllers;

[Route(HolidayTypesRoot)]
[ApiController]
public sealed class HolidayTypesController : ControllerBase {
    private const string HolidayTypesRoot = "api/tipos";
    private readonly IHolidayTypeService _holyTypeSvc;
    public HolidayTypesController(IHolidayTypeService holyTypeSvc) => _holyTypeSvc = holyTypeSvc;

    // ----- READ -----
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetHolidayType(int id, CancellationToken ct)
        => (await _holyTypeSvc.GetByIdAsync(id, ct))
                .ToApiResult(type => Ok(type));

    [HttpGet]
    public async Task<IActionResult> GetHolidayTypes(CancellationToken ct)
        => (await _holyTypeSvc.GetAllAsync(ct))
                .ToApiResult(types => Ok(types));

    // ----- CREATE -----
    [HttpPost]
    public async Task<IActionResult> CreateHolidayType(CreateHolidayTypeRequest request, CancellationToken ct)
        => (await _holyTypeSvc.AddAsync(request, ct))
                .ToApiResult(id => Created($"{HolidayTypesRoot}/{id}", id));

    // ----- UPDATE -----
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateHolidayType(int id, UpdateHolidayTypeRequest request, CancellationToken ct)
        => (await _holyTypeSvc.UpdateAsync(id, request, ct))
                .ToApiResult();

    // ----- DELETE -----
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteHolidayType(int id, CancellationToken ct) 
        => (await _holyTypeSvc.DeleteAsync(id, ct))
                .ToApiResult();
}