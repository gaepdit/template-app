using Microsoft.AspNetCore.Authorization;
using MyApp.Domain.Identity;

namespace MyApp.AppServices.Permissions.Requirements;

internal class AdminUserRequirement :
    AuthorizationHandler<AdminUserRequirement>, IAuthorizationRequirement
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdminUserRequirement requirement)
    {
        if (context.User.IsInRole(RoleName.Manager))
            context.Succeed(requirement);

        return Task.FromResult(0);
    }
}
