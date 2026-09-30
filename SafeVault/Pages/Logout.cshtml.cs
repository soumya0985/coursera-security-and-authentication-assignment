using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SafeVault.Services.Interfaces;
using System.Threading.Tasks;

namespace SafeVault.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly IUserAuthenticationService _authenticationService;

        public LogoutModel(
            IUserAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _authenticationService.LogoutAsync();

            return RedirectToPage("/Login");
        }
    }
}
