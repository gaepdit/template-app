using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyApp.AppServices.Customers;
using MyApp.AppServices.Customers.Dto;
using MyApp.AppServices.Customers.Permissions;
using MyApp.AppServices.Permissions;
using MyApp.WebApp.Models;
using MyApp.WebApp.Platform.PageModelHelpers;

namespace MyApp.WebApp.Pages.Customers;

[Authorize(Policy = nameof(Policies.AdminUser))]
public class RestoreModel : PageModel
{
    // Constructor
    private readonly ICustomerService _service;
    private readonly IAuthorizationService _authorization;

    public RestoreModel(
        ICustomerService service,
        IAuthorizationService authorization)
    {
        _service = service;
        _authorization = authorization;
    }

    // Properties
    [BindProperty]
    public Guid Id { get; set; }

    public CustomerViewDto Item { get; private set; } = default!;

    // Methods
    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id is null) return RedirectToPage("Index");
        var item = await _service.FindAsync(id.Value);
        if (item is null) return NotFound();

        Id = id.Value;
        Item = item;

        if (!await UserCanManageDeletionsAsync()) return Forbid();
        if (Item.IsDeleted) return Page();

        TempData.SetDisplayMessage(DisplayMessage.AlertContext.Info, "Customer is not deleted.");
        return RedirectToPage("Details", new { Item.Id });
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var item = await _service.FindAsync(Id);
        if (item is null) return NotFound();

        Item = item;
        if (!await UserCanManageDeletionsAsync()) return Forbid();

        if (!Item.IsDeleted)
        {
            TempData.SetDisplayMessage(DisplayMessage.AlertContext.Info, "Customer is not deleted.");
            return RedirectToPage("Details", new { Id });
        }

        if (!ModelState.IsValid) return Page();

        await _service.RestoreAsync(Id);
        TempData.SetDisplayMessage(DisplayMessage.AlertContext.Success, "Customer successfully restored.");
        return RedirectToPage("Details", new { Id });
    }

    private async Task<bool> UserCanManageDeletionsAsync() =>
        (await _authorization.AuthorizeAsync(User, Item, CustomerOperation.ManageDeletions)).Succeeded;
}
