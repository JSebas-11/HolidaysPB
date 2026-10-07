using HolidaysPB.Api.Common.Extensions;
using HolidaysPB.Core.Services.Holiday;
using Microsoft.AspNetCore.Mvc;

namespace HolidaysPB.Api.Controllers;

[Route(CalendarRoot)]
[ApiController]
public sealed class CalendarController : ControllerBase {
    private const string CalendarRoot = "api/calendario";
    private readonly IHolidayService _holidaySvc;
    public CalendarController(IHolidayService holidaySvc) => _holidaySvc = holidaySvc;

    // ----- READ -----
    [HttpGet("/festivos/{countryId:int}/")]
    public async Task<IActionResult> GetHolidays(int countryId, int? year, CancellationToken ct)
        => (await _holidaySvc.GetByCountryAsync(new HolidayFilterRequest(countryId, year), ct))
                .ToApiResult(holidays => Ok(holidays));
    
    // ----- VERIFICATION -----
    [HttpGet("/verificar/{countryId:int}/")]
    public async Task<IActionResult> Validate(int countryId, int year, int month, int day, CancellationToken ct)
        => (await _holidaySvc.IsHolidayAsync($"{year}-{month}-{day}", countryId, ct))
                .ToApiResult(value => Ok(value));
}