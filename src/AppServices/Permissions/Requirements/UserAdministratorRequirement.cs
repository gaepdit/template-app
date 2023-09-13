using Microsoft.AspNetCore.Authorization;
using MyApp.Domain.Identity;

namespace MyApp.AppServices.Permissions.Requirements;

internal class UserAdministratorRequirement :
    AuthorizationHandler<UserAdministratorRequirement>, IAuthorizationRequirement
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        UserAdministratorRequirement requirement)
    {
        if (context.User.IsInRole(RoleName.UserAdmin))
            context.Succeed(requirement);

        return Task.FromResult(0);
    }
}
