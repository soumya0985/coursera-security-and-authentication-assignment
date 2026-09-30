using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using SafeVault.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SafeVault.Pages.Admin;

[Authorize(Roles = "Admin")]
public class DashboardModel : PageModel
{
	private readonly ILogger<DashboardModel> _logger;
    private readonly IUserService _userService;
    internal List<IdentityUser> Users { get; private set; }

	public DashboardModel(ILogger<DashboardModel> logger, IUserService userService)
	{
		_logger = logger;
        _userService = userService;
        Users = new List<IdentityUser>();
	}

	public async Task<IActionResult> OnGet()
	{
		_logger.LogInformation("Admin dashboard accessed by user: {User}", User?.Identity?.Name ?? "Anonymous");
        Users = await _userService.GetUserListAsync();
		return Page();
	}

}
