﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyApp.AppServices.Customers;
using MyApp.AppServices.Customers.Dto;
using MyApp.AppServices.Permissions;
using MyApp.WebApp.Models;
using MyApp.WebApp.Platform.PageModelHelpers;

namespace MyApp.WebApp.Pages.Customers;

[Authorize(Policy = nameof(Policies.AdminUser))]
public class DeleteContactModel : PageModel
{
    // Constructor
    private readonly ICustomerService _service;
    private readonly IAuthorizationService _authorization;

    public DeleteContactModel(
        ICustomerService service,
        IAuthorizationService authorization)
    {
        _service = service;
        _authorization = authorization;
    }

    // Properties
    [BindProperty]
    public Guid ContactId { get; set; }

    public ContactViewDto ContactView { get; private set; } = default!;
    public CustomerSearchResultDto CustomerView { get; private set; } = default!;

    // Methods
    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id is null) return RedirectToPage("Index");

        var contact = await _service.FindContactAsync(id.Value);
        if (contact is null) return NotFound();
        ContactView = contact;

        var customer = await _service.FindBasicInfoAsync(contact.CustomerId);
        if (customer is null) return NotFound();
        CustomerView = customer;

        if (CustomerView.IsDeleted || !await UserCanManageDeletionsAsync())
            return NotFound();

        ContactId = id.Value;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid? id)
    {
        if (id is null || id != ContactId) return RedirectToPage("Index");

        var originalContact = await _service.FindContactAsync(id.Value);
        if (originalContact is null) return BadRequest();

        var customer = await _service.FindBasicInfoAsync(originalContact.CustomerId);
        if (customer is null) return BadRequest();
        CustomerView = customer;

        if (CustomerView.IsDeleted || !await UserCanManageDeletionsAsync() || !ModelState.IsValid)
            return BadRequest();

        await _service.DeleteContactAsync(ContactId);
        TempData.SetDisplayMessage(DisplayMessage.AlertContext.Success, "Contact successfully deleted.");
        return RedirectToPage("Details", new { CustomerView.Id });
    }

    private async Task<bool> UserCanManageDeletionsAsync() =>
        (await _authorization.AuthorizeAsync(User, nameof(Policies.AdminUser))).Succeeded;
}
