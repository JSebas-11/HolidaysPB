using HolidaysPB.Api.Common.Extensions;
using HolidaysPB.Core.Services.Holiday;
using Microsoft.AspNetCore.Mvc;

namespace HolidaysPB.Api.Controllers;

[Route(HolidaysRoot)]
[ApiController]
public sealed class HolidaysController : ControllerBase {
    private const string HolidaysRoot = "api/festivos";
    private readonly IHolidayService _holydaySvc;
    public HolidaysController(IHolidayService holydaySvc) => _holydaySvc = holydaySvc;

    // ----- READ -----
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetHoliday(int id, CancellationToken ct)
        => (await _holydaySvc.GetByIdAsync(id, ct))
                .ToApiResult(holiday => Ok(holiday));

    [HttpGet]
    public async Task<IActionResult> GetHolidays(CancellationToken ct)
        => (await _holydaySvc.GetAllAsync(ct))
                .ToApiResult(holidays => Ok(holidays));

    // ----- CREATE -----
    [HttpPost]
    public async Task<IActionResult> CreateHoliday(CreateHolidayRequest request, CancellationToken ct)
        => (await _holydaySvc.AddAsync(request, ct))
                .ToApiResult(id => Created($"{HolidaysRoot}/{id}", id));

    // ----- UPDATE -----
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateHoliday(int id, UpdateHolidayRequest request, CancellationToken ct)
        => (await _holydaySvc.UpdateAsync(id, request, ct))
                .ToApiResult();

    // ----- DELETE -----
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteHoliday(int id, CancellationToken ct) 
        => (await _holydaySvc.DeleteAsync(id, ct))
                .ToApiResult();
}