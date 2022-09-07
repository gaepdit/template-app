using MyAppRoot.AppServices.UserServices;
using MyAppRoot.Domain.Offices;
using MyAppRoot.Infrastructure.Contexts;
using MyAppRoot.Infrastructure.Identity;
using MyAppRoot.Infrastructure.Repositories;
using MyAppRoot.LocalRepository.Identity;
using MyAppRoot.LocalRepository.Repositories;
using MyAppRoot.WebApp.Platform.Settings;
using Microsoft.EntityFrameworkCore;

namespace MyAppRoot.WebApp.Platform.Program;

public static class DataServices
{
    public static void AddDataServices(this IServiceCollection services,
        ConfigurationManager configuration, bool isLocal)
    {
        // When running locally, you have the option to use in-memory data or build the database using LocalDB.
        if (isLocal && !ApplicationSettings.LocalDevSettings.BuildLocalDb)
        {
            services.AddDbContext<AppDbContext>(opts =>
                opts.UseInMemoryDatabase("TEMP_DB"));

            // Uses static data if no database is built.
            services.AddScoped<IUserService, LocalUserService>();
            services.AddSingleton<IOfficeRepository, LocalOfficeRepository>();
        }
        else
        {
            services.AddDbContext<AppDbContext>(opts =>
                opts.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    x => x.MigrationsAssembly("Infrastructure")));

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IOfficeRepository, OfficeRepository>();
        }
    }
}
