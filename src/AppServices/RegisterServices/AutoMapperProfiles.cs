using Microsoft.Extensions.DependencyInjection;
using MyAppRoot.AppServices.AutoMapper;

namespace MyAppRoot.AppServices.RegisterServices;

public static class AutoMapperProfiles
{
    public static void AddAutoMapperProfiles(this IServiceCollection services)
    {
        // Add AutoMapper profiles
        services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperProfile>());
    }
}
