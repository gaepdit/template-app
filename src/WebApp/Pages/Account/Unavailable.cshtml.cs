using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Enfo.WebApp.Pages.Account
{
    [AllowAnonymous]
    public class Unavailable : PageModel
    {
        [UsedImplicitly]
        public static void OnGet()
        {
            // Method intentionally left empty.
        }
    }
}