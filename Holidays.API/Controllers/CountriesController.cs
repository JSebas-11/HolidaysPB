using HolidaysPB.Api.Common.Extensions;
using HolidaysPB.Core.Services.Country;
using Microsoft.AspNetCore.Mvc;

namespace HolidaysPB.Api.Controllers;

[Route(CountriesRoot)]
[ApiController]
public sealed class CountriesController : ControllerBase {
    private const string CountriesRoot = "api/paises";
    private readonly ICountryService _countrySvc;
    public CountriesController(ICountryService countrySvc) => _countrySvc = countrySvc;

    // ----- READ -----
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCountry(int id, CancellationToken ct)
        => (await _countrySvc.GetByIdAsync(id, ct))
                .ToApiResult(country => Ok(country));

    [HttpGet]
    public async Task<IActionResult> GetCountries(CancellationToken ct)
        => (await _countrySvc.GetAllAsync(ct))
                .ToApiResult(countries => Ok(countries));

    // ----- CREATE -----
    [HttpPost]
    public async Task<IActionResult> CreateCountry(CreateCountryRequest request, CancellationToken ct)
        => (await _countrySvc.AddAsync(request, ct))
                .ToApiResult(id => Created($"{CountriesRoot}/{id}", id));

    // ----- UPDATE -----
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCountry(int id, UpdateCountryRequest request, CancellationToken ct) 
        => (await _countrySvc.UpdateAsync(id, request, ct))
                .ToApiResult();

    // ----- DELETE -----
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCountry(int id, CancellationToken ct) 
        => (await _countrySvc.DeleteAsync(id, ct))
                .ToApiResult();
}