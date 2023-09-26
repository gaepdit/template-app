using Microsoft.AspNetCore.Authorization;
using MyApp.AppServices.Permissions.Helpers;

namespace MyApp.AppServices.Permissions.Requirements;

internal class AdministrationViewRequirement :
    AuthorizationHandler<AdministrationViewRequirement>, IAuthorizationRequirement
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdministrationViewRequirement requirement)
    {
        if (context.User.IsStaffOrMaintainer())
            context.Succeed(requirement);

        return Task.FromResult(0);
    }
}
