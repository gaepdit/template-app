﻿using GaEpd.AppLibrary.Extensions;
using GaEpd.AppLibrary.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyApp.AppServices.Customers;
using MyApp.AppServices.Customers.Dto;
using MyApp.AppServices.Permissions;
using MyApp.Domain.Data;
using MyApp.WebApp.Models;
using MyApp.WebApp.Platform.Constants;

namespace MyApp.WebApp.Pages.Customers;

[Authorize(Policy = nameof(Policies.StaffUser))]
public class IndexModel : PageModel
{
    // Constructor
    private readonly ICustomerService _service;
    private readonly IAuthorizationService _authorization;

    public IndexModel(
        ICustomerService service,
        IAuthorizationService authorization)
    {
        _service = service;
        _authorization = authorization;
    }

    // Properties
    public CustomerSearchDto Spec { get; set; } = default!;
    public bool ShowResults { get; private set; }
    public IPaginatedResult<CustomerSearchResultDto> SearchResults { get; private set; } = default!;
    public string SortByName => Spec.Sort.ToString();
    public bool ShowDeletionSearchOptions { get; private set; }
    public PaginationNavModel PaginationNav => new(SearchResults, Spec.AsRouteValues());

    // Select lists
    public SelectList CountiesSelectList => new(Data.Counties);

    // Methods
    public async Task<IActionResult> OnGetAsync()
    {
        ShowDeletionSearchOptions = await UserCanManageDeletionsAsync();
        return Page();
    }

    public async Task<IActionResult> OnGetSearchAsync(CustomerSearchDto spec, [FromQuery] int p = 1)
    {
        Spec = spec.TrimAll();

        ShowDeletionSearchOptions = await UserCanManageDeletionsAsync();
        if (!ShowDeletionSearchOptions) Spec = Spec with { DeletedStatus = null };

        var paging = new PaginatedRequest(p, GlobalConstants.PageSize, Spec.Sort.GetDescription());
        SearchResults = await _service.SearchAsync(Spec, paging);

        ShowResults = true;
        return Page();
    }

    private async Task<bool> UserCanManageDeletionsAsync() =>
        (await _authorization.AuthorizeAsync(User, nameof(Policies.AdminUser))).Succeeded;
}
