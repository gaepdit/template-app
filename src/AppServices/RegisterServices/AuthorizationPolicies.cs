using Microsoft.Extensions.DependencyInjection;
using MyAppRoot.AppServices.Permissions;

namespace MyAppRoot.AppServices.RegisterServices;

public static class AuthorizationPolicies
{
    public static void AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(opts =>
        {
            opts.AddPolicy(PolicyName.SiteMaintainer, Policies.SiteMaintainerPolicy());
            opts.AddPolicy(PolicyName.UserAdministrator, Policies.UserAdministratorPolicy());
        });

        // services.AddSingleton<IAuthorizationHandler>(_ => new ComplaintViewPermissionsHandler());
    }
}
