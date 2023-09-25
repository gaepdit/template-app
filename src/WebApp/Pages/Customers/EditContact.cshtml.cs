using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyApp.AppServices.Customers;
using MyApp.AppServices.Customers.Dto;
using MyApp.AppServices.Customers.Permissions;
using MyApp.AppServices.Permissions;
using MyApp.Domain.Data;
using MyApp.WebApp.Models;
using MyApp.WebApp.Platform.PageModelHelpers;

namespace MyApp.WebApp.Pages.Customers;

[Authorize(Policy = nameof(Policies.StaffUser))]
public class EditContactModel : PageModel
{
    // Constructor
    private readonly ICustomerService _service;
    private readonly IValidator<ContactUpdateDto> _validator;
    private readonly IAuthorizationService _authorization;

    public EditContactModel(ICustomerService service, IValidator<ContactUpdateDto> validator,
        IAuthorizationService authorization)
    {
        _service = service;
        _validator = validator;
        _authorization = authorization;
    }

    // Properties
    [BindProperty]
    public ContactUpdateDto ContactUpdate { get; set; } = default!;

    [TempData]
    public Guid HighlightId { get; set; }

    public CustomerSearchResultDto CustomerView { get; private set; } = default!;
    public Dictionary<IAuthorizationRequirement, bool> UserCan { get; set; } = new();

    // Select lists
    public SelectList StatesSelectList => new(Data.States);

    // Messaging
    public Handlers Handler { get; private set; } = Handlers.None;

    public enum Handlers
    {
        None,
        EditContact,
    }

    // Methods
    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        // id is Contact ID
        if (id is null) return RedirectToPage("Index");

        var contact = await _service.FindContactForUpdateAsync(id.Value);
        if (contact is null) return NotFound();
        ContactUpdate = contact;

        var customer = await _service.FindBasicInfoAsync(contact.CustomerId);
        if (customer is null) return NotFound();
        CustomerView = customer;

        await SetPermissionsAsync(contact);

        if (UserCan[CustomerOperation.Edit])
            return Page();

        if (!UserCan[CustomerOperation.ManageDeletions])
            return NotFound();

        TempData.SetDisplayMessage(DisplayMessage.AlertContext.Info, "Cannot edit a deleted customer or contact.");
        return RedirectToPage("Details", new { id = ContactUpdate.CustomerId });
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var originalContact = await RefreshExistingData(ContactUpdate.Id);
        if (originalContact is null) return BadRequest();

        await SetPermissionsAsync(originalContact);
        if (!UserCan[CustomerOperation.Edit]) return BadRequest();

        Handler = Handlers.EditContact;

        await _validator.ApplyValidationAsync(ContactUpdate, ModelState);
        if (!ModelState.IsValid) return Page();

        await _service.UpdateContactAsync(ContactUpdate);

        HighlightId = ContactUpdate.Id;
        TempData.SetDisplayMessage(DisplayMessage.AlertContext.Success, "Contact successfully updated.");
        return RedirectToPage("Details", new { id = ContactUpdate.CustomerId });
    }

    private async Task SetPermissionsAsync(ContactUpdateDto item)
    {
        foreach (var operation in CustomerOperation.AllOperations)
            UserCan[operation] = (await _authorization.AuthorizeAsync(User, item, operation)).Succeeded;
    }

    private async Task<ContactUpdateDto?> RefreshExistingData(Guid id)
    {
        var originalContact = await _service.FindContactForUpdateAsync(id);
        if (originalContact is null) return null;

        var customer = await _service.FindBasicInfoAsync(originalContact.CustomerId);
        if (customer is null) return null;

        ContactUpdate.IsDeleted = originalContact.IsDeleted;
        ContactUpdate.CustomerIsDeleted = originalContact.CustomerIsDeleted;
        ContactUpdate.CustomerId = originalContact.CustomerId;

        CustomerView = customer;

        return originalContact;
    }
}
