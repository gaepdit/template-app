﻿using GaEpd.AppLibrary.Extensions;
using GaEpd.AppLibrary.ListItems;
using GaEpd.AppLibrary.Pagination;
using MyApp.AppServices.Offices;
using MyApp.AppServices.Permissions;
using MyApp.AppServices.Staff;
using MyApp.AppServices.Staff.Dto;
using MyApp.Domain.Identity;
using MyApp.WebApp.Models;
using MyApp.WebApp.Platform.Constants;

namespace MyApp.WebApp.Pages.Admin.Users;

[Authorize(Policy = nameof(Policies.ActiveUser))]
public class UsersIndexModel(IOfficeService officeService, IStaffService staffService) : PageModel
{
    public StaffSearchDto Spec { get; set; } = default!;
    public bool ShowResults { get; private set; }
    public IPaginatedResult<StaffSearchResultDto> SearchResults { get; private set; } = default!;
    public string SortByName => Spec.Sort.ToString();
    public PaginationNavModel PaginationNav => new(SearchResults, Spec.AsRouteValues());

    public SelectList RolesSelectList { get; private set; } = default!;
    public SelectList OfficesSelectList { get; private set; } = default!;

    public Task OnGetAsync() => PopulateSelectListsAsync();

    public async Task<IActionResult> OnGetSearchAsync(StaffSearchDto spec, [FromQuery] int p = 1)
    {
        Spec = spec.TrimAll();
        await PopulateSelectListsAsync();
        var paging = new PaginatedRequest(p, GlobalConstants.PageSize, Spec.Sort.GetDescription());
        SearchResults = await staffService.SearchAsync(Spec, paging);
        ShowResults = true;
        return Page();
    }

    private async Task PopulateSelectListsAsync()
    {
        OfficesSelectList = (await officeService.GetAsListItemsAsync(includeInactive: true)).ToSelectList();
        RolesSelectList = AppRole.AllRoles
            .Select(pair => new ListItem<string>(pair.Key, pair.Value.DisplayName))
            .ToSelectList();
    }
}
