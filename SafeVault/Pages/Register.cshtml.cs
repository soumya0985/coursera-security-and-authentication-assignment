using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SafeVault.Services.Interfaces;

namespace SafeVault.Pages;

public class RegisterModel : PageModel
{
    private readonly IUserRegistrationService _registrationService;

    public RegisterModel(IUserRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    [BindProperty]
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    public string Role { get; set; } = string.Empty;

    public void OnGet()
    {
        // Method intentionally left empty.
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (string.IsNullOrWhiteSpace(Username) ||
            string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Password) ||
            string.IsNullOrWhiteSpace(Role))
        {
            ModelState.AddModelError(string.Empty, "All fields are required.");
            return Page();
        }

        var result = await _registrationService.RegisterUserAsync(Username, Email, Password, Role);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
        
        // If we reach here, registration was successful.
        TempData["SuccessMessage"] = $"User '{Username}' has been registered successfully.";
        return RedirectToPage("/Index");
    }
}
