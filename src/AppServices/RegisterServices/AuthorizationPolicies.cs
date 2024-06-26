using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using MyApp.AppServices.WorkEntries.Permissions;
using MyApp.AppServices.Permissions;
using MyApp.AppServices.Permissions.AppClaims;
using System.Diagnostics.CodeAnalysis;

namespace MyApp.AppServices.RegisterServices;

[SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out")]
public static class AuthorizationPolicies
{
    public static void AddAuthorizationPolicies(this IServiceCollection services)
    {
        // These policies are for use in PageModel class attributes, e.g.:
        // [Authorize(Policy = nameof(Policies.ActiveUser))]

        services.AddAuthorizationBuilder()
            .AddPolicy(nameof(Policies.ActiveUser), Policies.ActiveUser)
            .AddPolicy(nameof(Policies.Manager), Policies.Manager)
            .AddPolicy(nameof(Policies.SiteMaintainer), Policies.SiteMaintainer)
            .AddPolicy(nameof(Policies.StaffUser), Policies.StaffUser)
            .AddPolicy(nameof(Policies.UserAdministrator), Policies.UserAdministrator);

        // Resource/operation-based permission handlers, e.g.:
        // var canAssign = await authorization.Succeeded(User, entryView, WorkEntryOperation.EditWorkEntry);

        services.AddSingleton<IAuthorizationHandler, WorkEntryViewRequirements>();

        // Add claims transformations
        services.AddScoped<IClaimsTransformation, AppClaimsTransformation>();
    }
}
