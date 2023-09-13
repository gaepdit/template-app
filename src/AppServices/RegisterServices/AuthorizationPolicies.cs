using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using MyApp.AppServices.Customers.Permissions;
using MyApp.AppServices.Permissions;

namespace MyApp.AppServices.RegisterServices;

public static class AuthorizationPolicies
{
    public static void AddAuthorizationPolicies(this IServiceCollection services)
    {
        // Authorization policies
        services.AddAuthorization(opts =>
        {
            opts.AddPolicy(nameof(Policies.AdministrationView), Policies.AdministrationView);
            opts.AddPolicy(nameof(Policies.ActiveUser), Policies.ActiveUser);
            opts.AddPolicy(nameof(Policies.AdminUser), Policies.AdminUser);
            opts.AddPolicy(nameof(Policies.LoggedInUser), Policies.LoggedInUser);
            opts.AddPolicy(nameof(Policies.SiteMaintainer), Policies.SiteMaintainer);
            opts.AddPolicy(nameof(Policies.StaffUser), Policies.StaffUser);
            opts.AddPolicy(nameof(Policies.UserAdministrator), Policies.UserAdministrator);
        });

        // Resource-based handlers
        services.AddSingleton<IAuthorizationHandler, ContactUpdatePermissionsHandler>();
        services.AddSingleton<IAuthorizationHandler, CustomerUpdatePermissionsHandler>();
        services.AddSingleton<IAuthorizationHandler, CustomerViewPermissionsHandler>();
    }
}
