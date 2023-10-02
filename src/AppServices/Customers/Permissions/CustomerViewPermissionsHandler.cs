using Microsoft.AspNetCore.Authorization;
using MyApp.AppServices.Customers.Dto;
using MyApp.AppServices.Permissions.Helpers;

namespace MyApp.AppServices.Customers.Permissions;

internal class CustomerViewPermissionsHandler :
    AuthorizationHandler<CustomerOperation, CustomerViewDto>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CustomerOperation requirement,
        CustomerViewDto resource)
    {
        if (!(context.User.Identity?.IsAuthenticated ?? false))
            return Task.FromResult(0);

        var success = requirement.Name switch
        {
            nameof(CustomerOperation.Edit) =>
                // Staff can edit.
                context.User.IsStaff() && IsNotDeleted(resource),

            nameof(CustomerOperation.ManageDeletions) =>
                // Only an Admin User can delete or restore.
                context.User.IsManager(),

            _ => false,
        };

        if (success) context.Succeed(requirement);
        return Task.FromResult(0);
    }

    private static bool IsNotDeleted(CustomerViewDto resource) => !resource.IsDeleted;
}
