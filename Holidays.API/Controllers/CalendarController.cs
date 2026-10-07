using HolidaysPB.Api.Common.Extensions;
using HolidaysPB.Core.Common.Result;
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
    [HttpGet("festivos/{countryId:int}/{year:int}")]
    public async Task<IActionResult> GetHolidays(int countryId, int year, CancellationToken ct)
        => (await _holidaySvc.GetByCountryAsync(new HolidayFilterRequest(countryId, year), ct))
                .ToApiResult(hds => Ok(hds));
    
    // ----- VERIFICATION -----
    [HttpGet("verificar/{countryId:int}/{year:int}/{month:int}/{day:int}")]
    public async Task<IActionResult> Validate(int countryId, int year, int month, int day, CancellationToken ct) {
        try {
            var date = new DateOnly(year, month, day);
            return (await _holidaySvc.IsHolidayAsync(date, countryId, ct))
                    .ToApiResult(value => Ok(value));
        }
        catch (ArgumentOutOfRangeException) {
            return Result.Fail(AppError.Validation("Provided date is not valid.")).ToApiResult();
        }
    }
}