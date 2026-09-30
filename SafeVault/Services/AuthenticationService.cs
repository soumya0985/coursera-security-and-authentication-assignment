using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SafeVault.Configuration;
using SafeVault.Helpers;
using SafeVault.Services.Interfaces;
using System.Threading.Tasks;

namespace SafeVault.Services;

public class AuthenticationService : IUserAuthenticationService
{
	private readonly SignInManager<IdentityUser> _signInManager;
	private readonly ValidationOptions _validationOptions;
	private readonly ILogger<AuthenticationService> _logger;

	public AuthenticationService(
		SignInManager<IdentityUser> signInManager,
		IOptions<ValidationOptions> validationOptions,
		ILogger<AuthenticationService> logger)
	{
		_signInManager = signInManager;
		_validationOptions = validationOptions.Value;
		_logger = logger;
	}

	public async Task<bool> LoginAsync(string username, string password)
	{
		if (!ValidationHelper.IsValidInput(username, _validationOptions.AllowedUsernameCharacters) ||
			string.IsNullOrWhiteSpace(password))
		{
			_logger.LogWarning("Login failed validation for username: {Username}", username);
			return false;
		}

		// ASP.NET Core Identity handles password hashing and validation requirements,
		// so this service only validates the username format before sign-in.
		_logger.LogInformation("Login attempt for username: {Username}", username);
		var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);
		if (result.Succeeded)
		{
			_logger.LogInformation("Login succeeded for username: {Username}", username);
		}
		else
		{
			_logger.LogWarning("Login failed for username: {Username}", username);
		}
		return result.Succeeded;
	}

	public async Task LogoutAsync()
	{
		_logger.LogInformation("User logout requested.");
		await _signInManager.SignOutAsync();
	}
}
