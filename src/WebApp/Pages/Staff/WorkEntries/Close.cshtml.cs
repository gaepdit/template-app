﻿using MyApp.AppServices.Permissions;
using MyApp.AppServices.Permissions.Helpers;
using MyApp.AppServices.WorkEntries;
using MyApp.AppServices.WorkEntries.CommandDto;
using MyApp.AppServices.WorkEntries.Permissions;
using MyApp.AppServices.WorkEntries.QueryDto;
using MyApp.WebApp.Models;
using MyApp.WebApp.Platform.PageModelHelpers;

namespace MyApp.WebApp.Pages.Staff.WorkEntries;

[Authorize(Policy = nameof(Policies.Manager))]
public class CloseModel(IWorkEntryService workEntryService, IAuthorizationService authorization) : PageModel
{
    [BindProperty]
    public WorkEntryChangeStatusDto EntryDto { get; set; } = null!;

    public WorkEntryViewDto ItemView { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id is null) return RedirectToPage("Index");

        var workEntryView = await workEntryService.FindAsync(id.Value);
        if (workEntryView is null) return NotFound();

        if (!await UserCanReviewAsync(workEntryView)) return Forbid();

        EntryDto = new WorkEntryChangeStatusDto(id.Value);
        ItemView = workEntryView;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return BadRequest();

        var workEntryView = await workEntryService.FindAsync(EntryDto.WorkEntryId);
        if (workEntryView is null || !await UserCanReviewAsync(workEntryView))
            return BadRequest();

        await workEntryService.CloseAsync(EntryDto);
        TempData.SetDisplayMessage(DisplayMessage.AlertContext.Success, "The Work Entry has been closed.");

        return RedirectToPage("Details", new { id = EntryDto.WorkEntryId });
    }

    private Task<bool> UserCanReviewAsync(WorkEntryViewDto item) =>
        authorization.Succeeded(User, item, WorkEntryOperation.EditWorkEntry);
}
