using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAppRoot.AppServices.Offices;
using MyAppRoot.AppServices.Permissions;
using MyAppRoot.WebApp.Models;
using MyAppRoot.WebApp.Platform.PageModelHelpers;

namespace MyAppRoot.WebApp.Pages.Admin.Maintenance.Offices;

[Authorize(Policy = PolicyName.SiteMaintainer)]
public class AddModel : PageModel
{
    // Constructor
    private readonly IOfficeService _service;
    private readonly IValidator<OfficeCreateDto> _validator;

    public AddModel(
        IOfficeService service,
        IValidator<OfficeCreateDto> validator)
    {
        _service = service;
        _validator = validator;
    }

    // Properties
    [BindProperty]
    public OfficeCreateDto Item { get; set; } = default!;

    [TempData]
    public Guid HighlightId { get; set; }

    public static MaintenanceOption ThisOption => MaintenanceOption.Office;

    // Methods
    public void OnGet()
    {
        // Method intentionally left empty.
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _validator.ApplyValidationAsync(Item, ModelState);
        if (!ModelState.IsValid) return Page();

        var id = await _service.CreateAsync(Item);

        HighlightId = id;
        TempData.SetDisplayMessage(DisplayMessage.AlertContext.Success, $"“{Item.Name}” successfully added.");
        return RedirectToPage("Index");
    }
}
