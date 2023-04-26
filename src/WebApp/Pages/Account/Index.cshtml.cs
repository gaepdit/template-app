using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAppRoot.AppServices.Staff;
using MyAppRoot.Domain.Identity;

namespace MyAppRoot.WebApp.Pages.Account;

[Authorize]
public class IndexModel : PageModel
{
    public StaffViewDto DisplayStaff { get; private set; } = default!;
    public string? OfficeName => DisplayStaff.Office?.Name;
    public IList<AppRole> Roles { get; private set; } = default!;

    public async Task<IActionResult> OnGetAsync([FromServices] IStaffAppService staffService)
    {
        var staff = await staffService.GetCurrentUserAsync();
        if (staff is not { Active: true }) return Forbid();

        DisplayStaff = staff;
        Roles = await staffService.GetAppRolesAsync(DisplayStaff.Id);

        return Page();
    }
}
