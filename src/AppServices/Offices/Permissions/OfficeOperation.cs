using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace MyAppRoot.AppServices.Offices.Permissions;

public class OfficeOperation : OperationAuthorizationRequirement // implements IAuthorizationRequirement
{
    public static readonly OfficeOperation ViewSelf = new() { Name = nameof(ViewSelf) };
}
