using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Entities.Contacts;
using MyApp.Domain.Entities.Customers;
using MyApp.Domain.Entities.Offices;
using MyApp.EfRepository.DbContext;
using MyApp.EfRepository.Repositories;
using MyApp.LocalRepository.Repositories;
using MyApp.WebApp.Platform.Settings;

namespace MyApp.WebApp.Platform.Services;

public static class DataStores
{
    public static void AddDataStores(this IServiceCollection services, ConfigurationManager configuration)
    {
        // When configured, use in-memory data; otherwise use a SQL Server database.
        if (ApplicationSettings.DevSettings.UseInMemoryData)
        {
            // Uses local static data if no database is built.
            services.AddSingleton<IContactRepository, LocalContactRepository>();
            services.AddSingleton<ICustomerRepository, LocalCustomerRepository>();
            services.AddSingleton<IOfficeRepository, LocalOfficeRepository>();
        }
        else
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                services.AddDbContext<AppDbContext>(opts => opts.UseInMemoryDatabase("TEMP_DB"));
            }
            else
            {
                services.AddDbContext<AppDbContext>(opts => opts.UseSqlServer(connectionString));
            }

            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IOfficeRepository, OfficeRepository>();
        }
    }
}
