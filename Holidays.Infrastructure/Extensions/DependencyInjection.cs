using HolidaysPB.Core.Persistence.Repositories;
using HolidaysPB.Core.Persistence.UnitOfWork;
using HolidaysPB.Domain.Entities;
using HolidaysPB.Infrastructure.Persitence;
using HolidaysPB.Infrastructure.Persitence.Repositories;
using HolidaysPB.Infrastructure.Persitence.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HolidaysPB.Infrastructure.Extensions;

public static class DependencyInjection {
    public static IServiceCollection AddHolidaysPBInfra(this IServiceCollection services, IConfiguration config) {
        var dbConn = config.GetConnectionString("DBConnection") 
            ?? throw new InvalidOperationException("DBConnection was not found.");

        services.AddDbContext<HolidaysDBContext>(opts => opts.UseSqlServer(dbConn));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IRepository<Country>, CountryRepository>();
        services.AddScoped<IRepository<HolidayType>, HolidayTypeRepository>();
        services.AddScoped<IHolidayRepository, HolidayRepository>();
        
        return services;
    }
}