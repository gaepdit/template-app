using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAppRoot.AppServices.StaffServices;
using MyAppRoot.AppServices.UserServices;
using MyAppRoot.Domain.Identity;
using MyAppRoot.WebApp.Models;
using MyAppRoot.WebApp.Platform.RazorHelpers;

namespace MyAppRoot.WebApp.Pages.Account;

[Authorize]
public class IndexModel : PageModel
{
    public StaffViewDto DisplayStaff { get; private set; } = default!;
    public string? OfficeName => DisplayStaff.Office?.Name;
    public IList<AppRole> Roles { get; private set; } = default!;
    public DisplayMessage? Message { get; private set; }

    public async Task<IActionResult> OnGetAsync(
        [FromServices] IUserService userService,
        [FromServices] IStaffAppService staffService)
    {
        var currentUser = await userService.GetCurrentUserAsync();
        if (currentUser is null) return Forbid();

        var staff = await staffService.FindAsync(currentUser.IdAsGuid);
        if (staff == null) return Forbid();

        DisplayStaff = staff;
        Roles = await staffService.GetAppRolesAsync(DisplayStaff.Id);
        Message = TempData.GetDisplayMessage();

        return Page();
    }
}
