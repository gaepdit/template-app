using Microsoft.Extensions.DependencyInjection;
using MyAppRoot.AppServices.Offices;
using MyAppRoot.Domain.Entities.Offices;

namespace MyAppRoot.AppServices.RegisterServices;

public static class AppServices
{
    public static void AddAppServices(this IServiceCollection services)
    {
        // Offices
        services.AddScoped<IOfficeManager, OfficeManager>();
        services.AddScoped<IOfficeAppService, OfficeAppService>();
    }
}
