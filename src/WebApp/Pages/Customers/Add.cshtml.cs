﻿using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyApp.AppServices.Customers;
using MyApp.AppServices.Customers.Dto;
using MyApp.AppServices.Permissions;
using MyApp.Domain.Data;
using MyApp.WebApp.Models;
using MyApp.WebApp.Platform.PageModelHelpers;

namespace MyApp.WebApp.Pages.Customers;

[Authorize(Policy = nameof(Policies.StaffUser))]
public class AddModel : PageModel
{
    // Constructor
    private readonly ICustomerService _service;
    private readonly IValidator<CustomerCreateDto> _validator;

    public AddModel(ICustomerService service, IValidator<CustomerCreateDto> validator)
    {
        _service = service;
        _validator = validator;
    }

    // Properties
    [BindProperty]
    public CustomerCreateDto Item { get; set; } = default!;

    // Select lists
    public SelectList StatesSelectList => new(Data.States);
    public SelectList CountiesSelectList => new(Data.Counties);

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
        TempData.SetDisplayMessage(DisplayMessage.AlertContext.Success, "New Customer successfully added.");
        return RedirectToPage("Details", new { id });
    }
}
