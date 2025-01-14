using MyApp.AppServices.Permissions;
using MyApp.AppServices.Staff;
using MyApp.AppServices.Staff.Dto;
using MyApp.Domain.Identity;

namespace MyApp.WebApp.Pages.Account;

[Authorize(Policy = nameof(Policies.ActiveUser))]
public class AccountIndexModel : PageModel
{
    public StaffViewDto DisplayStaff { get; private set; } = null!;
    public string? OfficeName => DisplayStaff.Office?.Name;
    public IReadOnlyList<AppRole> Roles { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync([FromServices] IStaffService staffService)
    {
        DisplayStaff = await staffService.GetCurrentUserAsync();
        Roles = await staffService.GetAppRolesAsync(DisplayStaff.Id);
        return Page();
    }
}
