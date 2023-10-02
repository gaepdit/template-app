using Microsoft.AspNetCore.Authorization;
using MyApp.AppServices.Customers.Dto;
using MyApp.AppServices.Permissions.Helpers;

namespace MyApp.AppServices.Customers.Permissions;

internal class CustomerUpdatePermissionsHandler :
    AuthorizationHandler<CustomerOperation, CustomerUpdateDto>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CustomerOperation requirement,
        CustomerUpdateDto resource)
    {
        if (!(context.User.Identity?.IsAuthenticated ?? false))
            return Task.FromResult(0);

        var success = requirement.Name switch
        {
            nameof(CustomerOperation.Edit) =>
                // Customers can only be edited if they are not deleted.
                context.User.IsStaff() && IsNotDeleted(resource),

            _ => false,
        };

        if (success) context.Succeed(requirement);
        return Task.FromResult(0);
    }

    private static bool IsNotDeleted(CustomerUpdateDto resource) => !resource.IsDeleted;
}
