using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyApp.AppServices.Permissions;
using MyApp.Domain.Identity;

namespace MyApp.WebApp.Pages;

[AllowAnonymous]
public class IndexModel : PageModel
{
    // Constructor
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAuthorizationService _authorization;

    public IndexModel(SignInManager<ApplicationUser> signInManager, IAuthorizationService authorization)
    {
        _signInManager = signInManager;
        _authorization = authorization;
    }

    // Properties
    public bool ShowDashboard { get; private set; }

    // Methods
    public async Task<IActionResult> OnGetAsync()
    {
        if (!_signInManager.IsSignedIn(User)) return LocalRedirect("~/Account/Login");

        ShowDashboard = await UseDashboardAsync();
        if (!ShowDashboard) return Page();

        // Load dashboard modules

        return Page();
    }

    private async Task<bool> UseDashboardAsync() =>
        (await _authorization.AuthorizeAsync(User, nameof(Policies.StaffUser))).Succeeded;
}
