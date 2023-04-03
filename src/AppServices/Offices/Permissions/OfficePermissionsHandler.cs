using Microsoft.AspNetCore.Authorization;

namespace MyAppRoot.AppServices.Offices.Permissions;

/// <summary>
/// For more info on resource-based authorization, see:
/// https://learn.microsoft.com/en-us/azure/architecture/multitenant-identity/authorize#resource-based-authorization
/// </summary>
public class OfficePermissionsHandler : AuthorizationHandler<OfficeOperation, OfficeUpdateDto>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OfficeOperation requirement,
        OfficeUpdateDto resource)
    {
        if (!(context.User.Identity?.IsAuthenticated ?? false))
            return Task.CompletedTask;

        if (requirement.Name == OfficeOperation.ViewSelf.Name &&
            resource.Id == resource.CurrentUserOfficeId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
