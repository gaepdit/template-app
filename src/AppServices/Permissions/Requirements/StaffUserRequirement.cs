using Microsoft.AspNetCore.Authorization;
using MyApp.Domain.Identity;

namespace MyApp.AppServices.Permissions.Requirements;

internal class StaffUserRequirement :
    AuthorizationHandler<StaffUserRequirement>, IAuthorizationRequirement
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        StaffUserRequirement requirement)
    {
        if (context.User.IsInRole(RoleName.Staff) || context.User.IsInRole(RoleName.Manager))
            context.Succeed(requirement);

        return Task.FromResult(0);
    }
}
