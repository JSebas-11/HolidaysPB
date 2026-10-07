using HolidaysPB.Api.Common.ErrorHandling;
using HolidaysPB.Application.Common.Extensions;
using HolidaysPB.Infrastructure.Extensions;

namespace HolidaysPB.Api.Common.Extensions;

internal static class DependencyInjection {
    public static IServiceCollection AddHolidaysPB(this IServiceCollection services, IConfiguration configuration) {
        services.AddHolidaysPBInfra(configuration);
        services.AddHolidaysPBApp();

        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }
}