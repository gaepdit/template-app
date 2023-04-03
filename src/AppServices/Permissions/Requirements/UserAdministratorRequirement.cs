using Microsoft.AspNetCore.Authorization;
using MyAppRoot.Domain.Identity;

namespace MyAppRoot.AppServices.Permissions.Requirements;

internal class UserAdministratorRequirement :
    AuthorizationHandler<UserAdministratorRequirement>, IAuthorizationRequirement
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        UserAdministratorRequirement requirement)
    {
        if (!(context.User.Identity?.IsAuthenticated ?? false))
            return Task.FromResult(0);

        if (context.User.IsInRole(RoleName.UserAdmin))
            context.Succeed(requirement);

        return Task.FromResult(0);
    }
}
