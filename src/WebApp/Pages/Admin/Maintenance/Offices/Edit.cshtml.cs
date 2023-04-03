using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAppRoot.AppServices.Offices;
using MyAppRoot.AppServices.Offices.Permissions;
using MyAppRoot.AppServices.Permissions;
using MyAppRoot.AppServices.Staff;
using MyAppRoot.WebApp.Models;
using MyAppRoot.WebApp.Platform.RazorHelpers;

namespace MyAppRoot.WebApp.Pages.Admin.Maintenance.Offices;

[Authorize(Policy = PolicyName.SiteMaintainer)]
public class EditModel : PageModel
{
    private readonly IOfficeAppService _service;
    private readonly IValidator<OfficeUpdateDto> _validator;
    private readonly IStaffAppService _staff;
    private readonly IAuthorizationService _authorization;

    public EditModel(
        IOfficeAppService service,
        IValidator<OfficeUpdateDto> validator,
        IStaffAppService staff,
        IAuthorizationService authorization)
    {
        _service = service;
        _validator = validator;
        _staff = staff;
        _authorization = authorization;
    }

    [BindProperty]
    public OfficeUpdateDto Item { get; set; } = default!;

    [BindProperty]
    public string OriginalName { get; set; } = string.Empty;

    public bool IsMyOffice { get; set; }

    [TempData]
    public Guid HighlightId { get; set; }

    public static MaintenanceOption ThisOption => MaintenanceOption.Office;

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
        var validationResult = await _validator.ValidateAsync(Item);
        if (!validationResult.IsValid) validationResult.AddToModelState(ModelState, nameof(Item));
        if (!ModelState.IsValid) return Page();

        await _service.UpdateAsync(Item);

        HighlightId = Item.Id;
        TempData.SetDisplayMessage(DisplayMessage.AlertContext.Success, $"“{Item.Name}” successfully updated.");
        return RedirectToPage("Index");
    }
}
