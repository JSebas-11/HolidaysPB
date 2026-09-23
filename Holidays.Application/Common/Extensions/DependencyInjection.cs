using HolidaysPB.Application.Features.Countries;
using HolidaysPB.Application.Features.Holidays;
using HolidaysPB.Application.Features.HolidayTypes;
using HolidaysPB.Core.Services;
using HolidaysPB.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace HolidaysPB.Application.Common.Extensions;

public static class DependencyInjection {
    public static IServiceCollection AddHolidaysPBApp(this IServiceCollection services) {
        services.AddScoped<ICrudService<Country>, CountryService>();
        services.AddScoped<ICrudService<HolidayType>, HolidayTypeService>();
        services.AddScoped<IHolidayService, HolidayService>();

        return services;
    }
}