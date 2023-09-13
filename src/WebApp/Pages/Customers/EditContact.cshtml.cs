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

    public EditContactModel(
        ICustomerService service,
        IValidator<ContactUpdateDto> validator,
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
    public DisplayMessage? PhoneNumberMessage { get; private set; }

    public enum Handlers
    {
        None,
        EditContact,
    }

    // Methods
    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id is null) return RedirectToPage("Index");

        var contact = await _service.FindContactForUpdateAsync(id.Value);
        if (contact is null) return NotFound();
        ContactUpdate = contact;

        var customer = await _service.FindBasicInfoAsync(contact.CustomerId);
        if (customer is null) return NotFound();
        CustomerView = customer;

        foreach (var operation in CustomerOperation.AllOperations) await SetPermissionAsync(operation);

        if (UserCan[CustomerOperation.Edit]) return Page();
        if (!UserCan[CustomerOperation.ManageDeletions]) return NotFound();

        TempData.SetDisplayMessage(DisplayMessage.AlertContext.Info, "Cannot edit a deleted customer or contact.");
        return RedirectToPage("Details", new { id = ContactUpdate.CustomerId });
    }

    public async Task<IActionResult> OnPostSaveContactAsync(Guid? id)
    {
        if (id is null || id != ContactUpdate.Id) return RedirectToPage("Index");
        if (!await RefreshExistingData(id.Value)) return BadRequest();

        foreach (var operation in CustomerOperation.AllOperations) await SetPermissionAsync(operation);
        if (!UserCan[CustomerOperation.Edit]) return Forbid();

        Handler = Handlers.EditContact;

        await _validator.ApplyValidationAsync(ContactUpdate, ModelState);
        if (!ModelState.IsValid) return Page();

        await _service.UpdateContactAsync(ContactUpdate);

        HighlightId = ContactUpdate.Id;
        TempData.SetDisplayMessage(DisplayMessage.AlertContext.Success, "Contact successfully updated.");
        return RedirectToPage("Details", null, new { id = ContactUpdate.CustomerId }, id.ToString());
    }

    private async Task SetPermissionAsync(IAuthorizationRequirement operation) =>
        UserCan[operation] = (await _authorization.AuthorizeAsync(User, ContactUpdate, operation)).Succeeded;

    private async Task<bool> RefreshExistingData(Guid id)
    {
        var originalContact = await _service.FindContactForUpdateAsync(id);
        if (originalContact is null) return false;
        ContactUpdate.IsDeleted = originalContact.IsDeleted;
        ContactUpdate.CustomerIsDeleted = originalContact.CustomerIsDeleted;
        ContactUpdate.CustomerId = originalContact.CustomerId;

        var customer = await _service.FindBasicInfoAsync(originalContact.CustomerId);
        if (customer is null) return false;

        CustomerView = customer;
        return true;
    }
}
