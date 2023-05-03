using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAppRoot.AppServices.Offices;
using MyAppRoot.AppServices.Offices.Permissions;
using MyAppRoot.AppServices.Permissions;
using MyAppRoot.AppServices.Staff;
using MyAppRoot.WebApp.Models;
using MyAppRoot.WebApp.Platform.PageModelHelpers;

namespace MyAppRoot.WebApp.Pages.Admin.Maintenance.Offices;

[Authorize(Policy = PolicyName.SiteMaintainer)]
public class EditModel : PageModel
{
    // Constructor
    private readonly IOfficeService _service;
    private readonly IValidator<OfficeUpdateDto> _validator;
    private readonly IStaffService _staff;
    private readonly IAuthorizationService _authorization;

    public EditModel(
        IOfficeService service,
        IValidator<OfficeUpdateDto> validator,
        IStaffService staff,
        IAuthorizationService authorization)
    {
        _service = service;
        _validator = validator;
        _staff = staff;
        _authorization = authorization;
    }

    // Properties
    [BindProperty]
    public OfficeUpdateDto Item { get; set; } = default!;

    [BindProperty]
    public string OriginalName { get; set; } = string.Empty;

    [TempData]
    public Guid HighlightId { get; set; }

    public bool IsMyOffice { get; set; }

    public static MaintenanceOption ThisOption => MaintenanceOption.Office;

    // Methods
    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        var staff = await _staff.GetCurrentUserAsync();
        if (staff is not { Active: true }) return Forbid();

        if (id is null) return RedirectToPage("Index");
        var item = await _service.FindForUpdateAsync(id.Value);
        if (item is null) return NotFound();

        Item = item;
        OriginalName = Item.Name;

        Item.CurrentUserOfficeId = staff.Office?.Id ?? Guid.Empty;
        IsMyOffice = (await _authorization.AuthorizeAsync(User, Item, OfficeOperation.ViewSelf)).Succeeded;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _validator.ApplyValidationAsync(Item, ModelState);
        if (!ModelState.IsValid) return Page();

        await _service.UpdateAsync(Item);

        HighlightId = Item.Id;
        TempData.SetDisplayMessage(DisplayMessage.AlertContext.Success, $"“{Item.Name}” successfully updated.");
        return RedirectToPage("Index");
    }
}
