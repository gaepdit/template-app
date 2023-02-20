using Microsoft.AspNetCore.Identity;
using MyAppRoot.AppServices.Staff;
using MyAppRoot.AppServices.UserServices;
using MyAppRoot.Domain.Identity;
using MyAppRoot.EfRepository.Contexts;
using MyAppRoot.LocalRepository.Identity;
using MyAppRoot.WebApp.Platform.Settings;

namespace MyAppRoot.WebApp.Platform.Services;

public static class IdentityStores
{
    public static void AddIdentityStores(this IServiceCollection services, bool isLocal)
    {
        var identityBuilder = services.AddIdentity<ApplicationUser, IdentityRole>();

        // When running locally, you have the option to use in-memory data or a database.
        if (isLocal && ApplicationSettings.LocalDevSettings.UseInMemoryData)
        {
            // Adds local UserStore and RoleSore
            services.AddSingleton<IUserStore<ApplicationUser>, LocalUserStore>();
            services.AddSingleton<IRoleStore<IdentityRole>, LocalRoleStore>();
        }
        else
        {
            // Add EF identity stores
            identityBuilder.AddRoles<IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
        }

        services.AddTransient<IStaffAppService, StaffAppService>();
        services.AddScoped<IUserService, UserService>();
    }
}
