using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyApp.AppServices.Customers;
using MyApp.AppServices.Customers.Dto;
using MyApp.AppServices.Customers.Permissions;
using MyApp.AppServices.Permissions;
using MyApp.WebApp.Models;
using MyApp.WebApp.Platform.PageModelHelpers;
using System.ComponentModel.DataAnnotations;

namespace MyApp.WebApp.Pages.Customers;

[Authorize(Policy = nameof(Policies.AdminUser))]
public class DeleteModel : PageModel
{
    // Constructor
    private readonly ICustomerService _service;
    private readonly IAuthorizationService _authorization;

    public DeleteModel(ICustomerService service, IAuthorizationService authorization)
    {
        _service = service;
        _authorization = authorization;
    }

    // Properties
    [BindProperty]
    public Guid Id { get; set; }

    [BindProperty]
    [Display(Name = "Deletion Comments (optional)")]
    public string? DeleteComments { get; set; }

    public CustomerViewDto Item { get; private set; } = default!;

    // Methods
    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id is null) return RedirectToPage("Index");
        var item = await _service.FindAsync(id.Value);
        if (item is null) return NotFound();
        if (!await UserCanManageDeletionsAsync(item)) return Forbid();

        if (item.IsDeleted)
        {
            TempData.SetDisplayMessage(DisplayMessage.AlertContext.Info, "Customer is already deleted.");
            return RedirectToPage("Details", new { id });
        }

        Id = id.Value;
        Item = item;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var item = await _service.FindAsync(Id);
        if (item is null) return BadRequest();
        if (!await UserCanManageDeletionsAsync(item)) return BadRequest();

        if (item.IsDeleted)
        {
            TempData.SetDisplayMessage(DisplayMessage.AlertContext.Info, "Customer is already deleted.");
            return RedirectToPage("Details", new { Id });
        }

        if (!ModelState.IsValid)
        {
            Item = item;
            return Page();
        }

        await _service.DeleteAsync(Id, DeleteComments);
        TempData.SetDisplayMessage(DisplayMessage.AlertContext.Success, "Customer successfully deleted.");
        return RedirectToPage("Details", new { Id });
    }

    private async Task<bool> UserCanManageDeletionsAsync(CustomerViewDto item) =>
        (await _authorization.AuthorizeAsync(User, item, CustomerOperation.ManageDeletions)).Succeeded;
}
