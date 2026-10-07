using HolidaysPB.Application.Features.Countries;
using HolidaysPB.Application.Features.Holidays;
using HolidaysPB.Application.Features.Holidays.Services;
using HolidaysPB.Application.Features.HolidayTypes;
using HolidaysPB.Core.Services.Country;
using HolidaysPB.Core.Services.Holiday;
using HolidaysPB.Core.Services.HolidayType;
using Microsoft.Extensions.DependencyInjection;

namespace HolidaysPB.Application.Common.Extensions;

public static class DependencyInjection {
    public static IServiceCollection AddHolidaysPBApp(this IServiceCollection services) {
        services.AddScoped<RelatedHolidayService>();
        
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<IHolidayTypeService, HolidayTypeService>();
        services.AddScoped<IHolidayService, HolidayService>();

        return services;
    }
}