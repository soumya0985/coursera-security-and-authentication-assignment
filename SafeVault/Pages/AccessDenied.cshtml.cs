using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SafeVault.Pages;

public class AccessDeniedModel : PageModel
{
    public void OnGet()
    {
        // This page is displayed when a user tries to access a resource they do not have permission for.
    }
}