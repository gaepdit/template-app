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

    public DetailsModel(ICustomerService customers, IStaffService staff, IAuthorizationService authorization)
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
        if (id is null) return RedirectToPage("../Index");
        var item = await _customers.FindAsync(id.Value);
        if (item is null) return NotFound();

        await SetPermissionsAsync(item);
        if (item.IsDeleted && !UserCan[CustomerOperation.ManageDeletions])
            return NotFound();

        Item = item;
        return Page();
    }

    private async Task SetPermissionsAsync(CustomerViewDto item)
    {
        foreach (var operation in CustomerOperation.AllOperations)
            UserCan[operation] = (await _authorization.AuthorizeAsync(User, item, operation)).Succeeded;
    }
}
