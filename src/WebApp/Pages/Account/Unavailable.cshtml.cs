using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyAppRoot.WebApp.Pages.Account;

[AllowAnonymous]
public class UnavailableModel : PageModel
{
    public static void OnGet()
    {
        // Method intentionally left empty.
    }
}
