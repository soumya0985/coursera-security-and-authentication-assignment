using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SafeVault.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SafeVault.Pages;

public class LoginModel : PageModel
{
	private readonly IUserAuthenticationService _authenticationService;

	public LoginModel(IUserAuthenticationService authenticationService)
	{
		_authenticationService = authenticationService;
	}

	[BindProperty]
	[Required]
	public string Username { get; set; } = string.Empty;

	[BindProperty]
	[Required]
	public string Password { get; set; } = string.Empty;

	public string? StatusMessage { get; set; }
	public bool IsSuccess { get; set; }

	public void OnGet()
    {
        // This page is displayed when the user navigates to the login page.
    }

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid)
		{
			return Page();
		}

        if (string.IsNullOrWhiteSpace(Username) ||
            string.IsNullOrWhiteSpace(Password))
        {
            ModelState.AddModelError(string.Empty, "All fields are required.");
            StatusMessage = "All fields are required.";
            IsSuccess = false;
            return Page();
        }

		var isAuthenticated = await _authenticationService.LoginAsync(Username, Password);

		if (isAuthenticated)
		{
			return RedirectToPage("/Index");
		}

		ModelState.AddModelError(string.Empty, "Invalid username or password.");
		StatusMessage = "Invalid username or password.";
		IsSuccess = false;
		return Page();
	}
}
