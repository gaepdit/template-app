using Microsoft.AspNetCore.Authorization;
using MyApp.Domain.Identity;

namespace MyApp.AppServices.Permissions.Requirements;

internal class AdministrationViewRequirement :
    AuthorizationHandler<AdministrationViewRequirement>, IAuthorizationRequirement
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdministrationViewRequirement requirement)
    {
        if (context.User.IsInRole(RoleName.Staff) ||
            context.User.IsInRole(RoleName.SiteMaintenance) ||
            context.User.IsInRole(RoleName.Manager))
            context.Succeed(requirement);

        return Task.FromResult(0);
    }
}
