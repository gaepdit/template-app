using Microsoft.AspNetCore.Authorization;
using MyApp.Domain.Identity;

namespace MyApp.AppServices.Permissions.Requirements;

internal class SiteMaintainerRequirement :
    AuthorizationHandler<SiteMaintainerRequirement>, IAuthorizationRequirement
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SiteMaintainerRequirement requirement)
    {
        if (context.User.IsInRole(RoleName.SiteMaintenance))
            context.Succeed(requirement);

        return Task.FromResult(0);
    }
}
