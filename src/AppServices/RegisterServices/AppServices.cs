using Microsoft.Extensions.DependencyInjection;
using MyApp.AppServices.Customers;
using MyApp.AppServices.Offices;
using MyApp.Domain.Entities.Customers;
using MyApp.Domain.Entities.Offices;

namespace MyApp.AppServices.RegisterServices;

public static class AppServices
{
    public static void AddAppServices(this IServiceCollection services)
    {
        // Customers (and Contacts)
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ICustomerManager, CustomerManager>();

        // Offices
        services.AddScoped<IOfficeManager, OfficeManager>();
        services.AddScoped<IOfficeService, OfficeService>();
    }
}
