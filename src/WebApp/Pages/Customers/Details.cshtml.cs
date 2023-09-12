using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyApp.AppServices.Customers;
using MyApp.AppServices.Customers.Dto;
using MyApp.AppServices.Customers.Permissions;
using MyApp.AppServices.Permissions;
using MyApp.AppServices.Staff;

namespace MyApp.WebApp.Pages.Customers;

[Authorize(Policy = nameof(Policies.StaffUser))]
public class DetailsModel : PageModel
{
    // Constructor
    private readonly ICustomerService _customers;
    private readonly IStaffService _staff;
    private readonly IAuthorizationService _authorization;

    public DetailsModel(
        ICustomerService customers,
        IStaffService staff,
        IAuthorizationService authorization)
    {
        _customers = customers;
        _staff = staff;
        _authorization = authorization;
    }

    // Properties
    [TempData]
    public Guid HighlightId { get; set; }

    public CustomerViewDto Item { get; private set; } = default!;
    public Dictionary<IAuthorizationRequirement, bool> UserCan { get; set; } = new();

    // Methods
    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (await _staff.GetCurrentUserAsync() is not { Active: true }) return Forbid();

        if (id is null) return RedirectToPage("../Index");
        var item = await _customers.FindAsync(id.Value);
        if (item is null) return NotFound();

        Item = item;

        foreach (var operation in CustomerOperation.AllOperations) await SetPermissionAsync(operation);
        if (Item.IsDeleted && !UserCan[CustomerOperation.ManageDeletions]) return Forbid();

        return Page();
    }

    private async Task SetPermissionAsync(IAuthorizationRequirement operation) =>
        UserCan[operation] = (await _authorization.AuthorizeAsync(User, Item, operation)).Succeeded;
}
