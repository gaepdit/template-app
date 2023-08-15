using Microsoft.AspNetCore.Authorization;
using MyAppRoot.AppServices.Permissions.Requirements;

namespace MyAppRoot.AppServices.Permissions;

public static class PolicyName
{
    public const string UserAdministrator = nameof(UserAdministrator);
    public const string SiteMaintainer = nameof(SiteMaintainer);
}

public static class Policies
{
    public static AuthorizationPolicy SiteMaintainerPolicy() =>
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new SiteMaintainerRequirement())
            .Build();

    public static AuthorizationPolicy UserAdministratorPolicy() =>
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new UserAdministratorRequirement())
            .Build();
}
