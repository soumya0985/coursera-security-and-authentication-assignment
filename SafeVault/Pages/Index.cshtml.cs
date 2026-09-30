using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using SafeVault.Services.Interfaces;

namespace SafeVault.Pages;

public class IndexModel : PageModel
{
    private readonly IUserRegistrationService _registrationService;

    public IndexModel(IUserRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }
    
    public string StatusMessage { get; private set; } = string.Empty;
    public bool IsSuccess { get; private set; }

    public async Task OnGet()
    {
        //Nothing to do on GET for now
    }

    public async Task<IActionResult> OnPost(string username, string email)
    {
        if (!await _registrationService.CheckRegisteredUserAsync(username, email))
        {
            IsSuccess = false;
            StatusMessage = "Invalid input.";
            ModelState.AddModelError(string.Empty, StatusMessage);
        }
        else
        {
                IsSuccess = true;
                StatusMessage = "Username and email combination found.";
                ModelState.Clear();
        }
        await OnGet();
        return Page();
    }


}
