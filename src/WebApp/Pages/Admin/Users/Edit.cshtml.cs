﻿using FluentValidation;
using GaEpd.AppLibrary.ListItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyApp.AppServices.Offices;
using MyApp.AppServices.Permissions;
using MyApp.AppServices.Staff;
using MyApp.AppServices.Staff.Dto;
using MyApp.WebApp.Models;
using MyApp.WebApp.Platform.PageModelHelpers;

namespace MyApp.WebApp.Pages.Admin.Users;

[Authorize(Policy = nameof(Policies.UserAdministrator))]
public class EditModel : PageModel
{
    // Constructor
    private readonly IStaffService _staffService;
    private readonly IOfficeService _officeService;
    private readonly IValidator<StaffUpdateDto> _validator;

    public EditModel(
        IStaffService staffService,
        IOfficeService officeService,
        IValidator<StaffUpdateDto> validator)
    {
        _staffService = staffService;
        _officeService = officeService;
        _validator = validator;
    }

    // Properties

    [FromRoute]
    public Guid Id { get; set; }

    [BindProperty]
    public StaffUpdateDto UpdateStaff { get; set; } = default!;

    public StaffViewDto DisplayStaff { get; private set; } = default!;

    // Select lists
    public SelectList OfficeItems { get; private set; } = default!;

    // Methods
    public async Task<IActionResult> OnGetAsync(string? id)
    {
        if (id is null) return RedirectToPage("Index");
        if (!Guid.TryParse(id, out var guid)) return NotFound();

        var staff = await _staffService.FindAsync(id);
        if (staff is null) return NotFound();

        Id = guid;
        DisplayStaff = staff;
        UpdateStaff = DisplayStaff.AsUpdateDto();

        await PopulateSelectListsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _validator.ApplyValidationAsync(UpdateStaff, ModelState);

        if (!ModelState.IsValid)
        {
            var staff = await _staffService.FindAsync(Id.ToString());
            if (staff is null) return BadRequest();

            DisplayStaff = staff;

            await PopulateSelectListsAsync();
            return Page();
        }

        var result = await _staffService.UpdateAsync(Id.ToString(), UpdateStaff);
        if (!result.Succeeded) return BadRequest();

        TempData.SetDisplayMessage(DisplayMessage.AlertContext.Success, "Successfully updated.");
        return RedirectToPage("Details", new { Id });
    }

    private async Task PopulateSelectListsAsync() =>
        OfficeItems = (await _officeService.GetActiveListItemsAsync()).ToSelectList();
}
